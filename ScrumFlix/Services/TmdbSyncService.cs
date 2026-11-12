/*
 * File: /ScrumFlix/Services/TmdbSyncService.cs
 * Namespace: ScrumFlix.Services
 * Purpose: Synchronizes movie metadata and genre data from the TMDb API into
 *          the canonical MovieTmdbMetadata, Genres, and MovieGenres tables.
 *
 * DEPENDENCIES:
 *   TMDbLib 4.0.0         — TMDbClient (NuGet)
 *   AppDbContext           — EF Core DbContext
 *   ILogger<T>             — Serilog via ILogger pipeline
 *   IConfiguration         — reads "Tmdb:ApiKey" from User Secrets / env vars
 *
 * TMDB API KEY:
 *   Set via User Secrets (development) or environment variable (production):
 *     dotnet user-secrets set "Tmdb:ApiKey" "your_v3_api_key_here"
 *   Production env var: Tmdb__ApiKey (double-underscore for nested config)
 *   The key is read once at construction time. If missing, the service logs
 *   a critical error and all sync operations return immediately without
 *   throwing — safe degradation, app continues without poster enrichment.
 *
 * HTTP TRANSPORT:
 *   This scoped service constructs TMDbClient(apiKey) directly. The separately
 *   registered named HttpClient "TmdbClient" is unused by this service; its
 *   resilience handler does not apply to these TMDbLib calls. The June 12, 2026
 *   hang investigation suggested a transport change, but did not prove a root
 *   cause. Subsequent queue/progress fixes and the TMDbLib 4.0.0 upgrade are
 *   separate changes; preserve this working path unless a real failure warrants
 *   new diagnostics. No transport-specific timeout/retry guarantee is asserted.
 *
 * SYNC STRATEGY:
 *   1. SyncGenresAsync   — upsert the TMDb genre master list into Genres.
 *   2. SyncAllMoviesAsync — for each Movie in Movies table:
 *      a. Refresh an existing match by its stored TMDbMovieId.
 *      b. For an unmatched movie, search by title and require one exact match.
 *         Ambiguous titles need a reviewed TMDb ID before syncing.
 *      c. Fetch full movie detail with Videos flag to get trailer keys.
 *      d. Upsert MovieTmdbMetadata (insert or update by MovieId).
 *      e. Replace MovieGenres for this movie from the TMDb genre_ids list.
 *      f. Update LastSyncedUtc; skip movies synced within 24h unless forceAll.
 *
 * GENRE UPSERT:
 *   Genres are inserted if TMDbGenreId is new, updated if Name has changed.
 *   The Slug is generated as lowercase-hyphenated from Name on first insert
 *   and never updated after that (slugs appear in URLs — changing them breaks links).
 *
 * TRAILER SELECTION:
 *   TMDb videos include trailers, teasers, clips, featurettes, etc.
 *   We select the first Video where Type == "Trailer" AND Site == "YouTube".
 *   If none exists, TrailerYouTubeKey is set to null.
 *
 * STALE CHECK:
 *   A movie is stale if MovieTmdbMetadata does not exist or LastSyncedUtc is
 *   more than 24 hours ago. This matches MovieTmdbMetadata.IsStale computed property.
 *
 * ERROR HANDLING:
 *   Per-movie errors are caught, logged at Warning level, counted in Failed,
 *   and skipped — one bad movie title must not abort the whole sync.
 *   The genre sync is atomic — any failure there is logged at Error.
 *
 * CSP NOTE:
 *   TMDb poster images are served from image.tmdb.org. If ImageSharp.Web is
 *   proxying them via /images/, the CSP img-src directive in
 *   SecurityHeadersConfiguration.cs does NOT need image.tmdb.org — the proxy
 *   serves from 'self'. If you render PosterUrl directly (no proxy), add
 *   image.tmdb.org to img-src.
 *
 * Phase: 2 (new)
 * Author: ScrumFlix Rebuild Team
 */

using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace ScrumFlix.Services;

/// <summary>
/// Synchronizes movie metadata (posters, trailers, ratings, genres) from
/// the TMDb API into the MovieTmdbMetadata, Genres, and MovieGenres tables.
/// Registered as scoped by TmdbConfiguration. The Admin action uses it for
/// single-movie requests and queued full-catalog sync operations.
/// </summary>
public sealed class TmdbSyncService : ITmdbSyncService
{
    // ── Constants ────────────────────────────────────────────────────────────

    /// <summary>Hours before a metadata record is considered stale and eligible for re-sync.</summary>
    private const int StaleThresholdHours = 24;

    /// <summary>Maximum milliseconds to wait between per-movie API calls to respect TMDb rate limits.</summary>
    private const int RateLimitDelayMs = 250;

    // ── Dependencies ─────────────────────────────────────────────────────────

    private readonly AppDbContext              _db;
    private readonly ILogger<TmdbSyncService>  _logger;
    private readonly TMDbClient?               _tmdb;   // null if API key is missing

    /// <summary>
    /// Initializes TmdbSyncService. Reads the TMDb API key from configuration.
    /// If the key is absent, _tmdb remains null and all sync methods return
    /// immediately — the app degrades gracefully without poster data.
    /// </summary>
    public TmdbSyncService(
        AppDbContext                    db,
        ILogger<TmdbSyncService>        logger,
        IConfiguration                  configuration)
    {
        _db     = db;
        _logger = logger;

        var apiKey = configuration["Tmdb:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogCritical(
                "TMDb API key is not configured. Set 'Tmdb:ApiKey' in User Secrets or " +
                "the Tmdb__ApiKey environment variable. " +
                "Movie poster and genre syncing will be disabled until the key is provided.");
            return;
        }

        _tmdb = new TMDbClient(apiKey);
    }

    // ── Public interface ─────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task<int> SyncGenresAsync(CancellationToken cancellationToken = default)
    {
        if (_tmdb is null)
        {
            _logger.LogWarning("SyncGenresAsync skipped — TMDb client not initialized (missing API key).");
            return 0;
        }

        _logger.LogInformation("TmdbSyncService: starting genre sync.");

        try
        {
            // Fetch English genre list from TMDb
            var tmdbGenres = await _tmdb.GetMovieGenresAsync(cancellationToken: cancellationToken);

            if (tmdbGenres == null || tmdbGenres.Count == 0)
            {
                _logger.LogWarning("SyncGenresAsync: TMDb returned an empty genre list.");
                return 0;
            }

            int upserted = 0;

            foreach (var tmdbGenre in tmdbGenres)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var existing = await _db.Genres
                    .FirstOrDefaultAsync(g => g.TMDbGenreId == tmdbGenre.Id, cancellationToken);

                if (existing is null)
                {
                    // New genre — insert with generated slug
                    _db.Genres.Add(new Genre
                    {
                        TMDbGenreId = tmdbGenre.Id,
                        // tmdbGenre.Name may be null coming from TMDb library; guard against it
                        Name        = tmdbGenre.Name ?? string.Empty,
                        Slug        = GenerateSlug(tmdbGenre.Name),
                        IsActive    = true,
                        CreatedUtc  = DateTime.UtcNow
                    });
                    upserted++;
                }
                else if (!string.IsNullOrWhiteSpace(tmdbGenre.Name) &&
                         !string.Equals(existing.Name, tmdbGenre.Name, StringComparison.Ordinal))
                {
                    // Name changed on TMDb — update display name but NOT the slug
                    // (slug changes break existing URLs). Only update if TMDb provided a non-null name.
                    existing.Name = tmdbGenre.Name!;
                    upserted++;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "TmdbSyncService: genre sync complete — {Count} genre(s) added or updated.",
                upserted);

            return upserted;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("SyncGenresAsync cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncGenresAsync failed with an unexpected error.");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<TmdbSyncResult> SyncAllMoviesAsync(
        bool                               forceAll          = false,
        IProgress<TmdbSyncProgressReport>? progress          = null,
        CancellationToken                  cancellationToken = default)
    {
        if (_tmdb is null)
        {
            _logger.LogWarning("SyncAllMoviesAsync skipped — TMDb client not initialized (missing API key).");
            return new TmdbSyncResult(0, 0, 0);
        }

        _logger.LogInformation(
            "TmdbSyncService: starting full catalog sync (forceAll={ForceAll}).", forceAll);

        // Load all movies with their existing metadata (if any) in one query
        var movies = await _db.Movies
            .Include(m => m.TmdbMetadata)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        int synced = 0, skipped = 0, failed = 0;
        int total = movies.Count;
        int processed = 0;

        foreach (var movie in movies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Skip if fresh and not forced
            if (!forceAll && movie.TmdbMetadata is { IsStale: false })
            {
                skipped++;
            }
            else
            {
                var success = await SyncMovieCoreAsync(movie, cancellationToken);
                if (success) synced++;
                else         failed++;

                // Respect TMDb rate limit (40 requests/10s default free tier)
                await Task.Delay(RateLimitDelayMs, cancellationToken);
            }

            processed++;

            // Report real progress after each movie — drives the sf-spinner on TmdbSyncPage
            if (progress is not null)
            {
                var percent = total == 0 ? 100 : (int)Math.Round((double)processed / total * 100);
                var msg = failed > 0
                    ? $"Syncing '{movie.Title}'… {synced} done, {failed} failed ({processed}/{total})"
                    : $"Syncing '{movie.Title}'… {processed}/{total}";

                progress.Report(new TmdbSyncProgressReport(
                    Percent: percent,
                    Message: msg,
                    Synced:  synced,
                    Skipped: skipped,
                    Failed:  failed,
                    Total:   total));
            }
        }

        _logger.LogInformation(
            "TmdbSyncService: full catalog sync complete — " +
            "Synced={Synced}, Skipped={Skipped}, Failed={Failed}, Total={Total}.",
            synced, skipped, failed, synced + skipped + failed);

        return new TmdbSyncResult(synced, skipped, failed);
    }

    /// <inheritdoc/>
    public async Task<bool> SyncMovieAsync(int movieId, CancellationToken cancellationToken = default)
    {
        if (_tmdb is null)
        {
            _logger.LogWarning("SyncMovieAsync({MovieId}) skipped — TMDb client not initialized.", movieId);
            return false;
        }

        var movie = await _db.Movies
            .Include(m => m.TmdbMetadata)
            .FirstOrDefaultAsync(m => m.MovieId == movieId, cancellationToken);

        if (movie is null)
        {
            _logger.LogWarning("SyncMovieAsync: MovieId {MovieId} not found in local database.", movieId);
            return false;
        }

        return await SyncMovieCoreAsync(movie, cancellationToken);
    }

    // ── Core sync logic ──────────────────────────────────────────────────────

    /// <summary>
    /// Performs the full TMDb sync for a single movie:
    ///   1. Reuse an existing TMDb ID, or find one unambiguous exact-title match.
    ///   2. Fetch full movie detail (with Videos for trailer key).
    ///   3. Upsert MovieTmdbMetadata row.
    ///   4. Replace MovieGenres for this movie.
    /// Returns true on success, false on any per-movie failure.
    /// </summary>
    private async Task<bool> SyncMovieCoreAsync(Domain.Movie movie, CancellationToken cancellationToken)
    {
        var client = _tmdb;
        if (client is null)
        {
            _logger.LogWarning("TmdbSyncService: cannot sync MovieId={MovieId} without a TMDb client.", movie.MovieId);
            return false;
        }

        try
        {
            _logger.LogDebug("TmdbSyncService: syncing '{Title}' (MovieId={MovieId}).",
                movie.Title, movie.MovieId);

            // ── Step 1: Keep the stored ID stable across subsequent syncs. ────
            // ReleaseDate and OriginalTitle are fetched enrichment, not a reliable
            // source of truth for identifying the movie. Searching on every refresh
            // can silently switch between different films with the same title.
            var tmdbId = movie.TmdbMetadata?.TMDbMovieId ?? 0;
            if (tmdbId <= 0)
            {
                var searchResults = await client.SearchMovieAsync(
                    query: movie.Title,
                    cancellationToken: cancellationToken);

                var exactMatches = searchResults?.Results?
                    .Where(r => string.Equals(r.Title, movie.Title,
                                 StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (exactMatches is null || exactMatches.Count != 1)
                {
                    _logger.LogWarning(
                        "TmdbSyncService: '{Title}' (MovieId={MovieId}) has {MatchCount} exact TMDb " +
                        "matches on the first page. Review the release year and assign a TMDb ID " +
                        "before syncing this movie.",
                        movie.Title, movie.MovieId, exactMatches?.Count ?? 0);
                    return false;
                }

                tmdbId = exactMatches[0].Id;
            }

            // ── Step 2: Fetch full detail with Videos ─────────────────────────
            // MovieMethods.Videos fetches the video list (trailers, teasers, etc.)
            // in a single API call — no second round-trip needed.
            var detail = await client.GetMovieAsync(
                movieId:      tmdbId,
                extraMethods: MovieMethods.Videos,
                cancellationToken: cancellationToken);

            if (detail is null)
            {
                _logger.LogWarning(
                    "TmdbSyncService: GetMovieAsync returned null for TMDbMovieId={TMDbId} " +
                    "(Movie: '{Title}', MovieId={MovieId}).",
                    tmdbId, movie.Title, movie.MovieId);
                return false;
            }

            // Extract the first YouTube trailer key (Type="Trailer", Site="YouTube")
            var trailerKey = detail.Videos?.Results
                ?.FirstOrDefault(v =>
                    string.Equals(v.Type, "Trailer", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(v.Site, "YouTube", StringComparison.OrdinalIgnoreCase))
                ?.Key;

            // ── Step 3: Upsert MovieTmdbMetadata ─────────────────────────────
            var now = DateTime.UtcNow;

            var metadata = await _db.MovieTmdbMetadata
                .FirstOrDefaultAsync(m => m.MovieId == movie.MovieId, cancellationToken);

            if (metadata is null)
            {
                // First sync for this movie — insert
                metadata = new MovieTmdbMetadata
                {
                    MovieId    = movie.MovieId,
                    CreatedUtc = now
                };
                _db.MovieTmdbMetadata.Add(metadata);
            }
            else
            {
                // Subsequent sync — track as modified
                _db.MovieTmdbMetadata.Attach(metadata);
                _db.Entry(metadata).State = EntityState.Modified;
            }

            // Map TMDb fields → domain model
            metadata.TMDbMovieId       = detail.Id;
            metadata.PosterPath        = detail.PosterPath;
            metadata.BackdropPath      = detail.BackdropPath;
            metadata.TrailerYouTubeKey = trailerKey;
            metadata.OriginalTitle     = detail.OriginalTitle;
            metadata.OriginalLanguage  = detail.OriginalLanguage;
            metadata.ReleaseDate       = detail.ReleaseDate.HasValue
                                            ? DateOnly.FromDateTime(detail.ReleaseDate.Value)
                                            : null;
            metadata.Popularity        = (decimal?)detail.Popularity;
            metadata.VoteAverage       = (decimal?)detail.VoteAverage;
            metadata.VoteCount         = detail.VoteCount;
            metadata.LastSyncedUtc     = now;
            metadata.UpdatedUtc        = now;

            // ── Step 4: Replace MovieGenres ───────────────────────────────────
            await SyncMovieGenresAsync(movie.MovieId, detail.Genres, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "TmdbSyncService: synced '{Title}' (MovieId={MovieId}, TMDbMovieId={TMDbId}) — " +
                "Poster={HasPoster}, Trailer={HasTrailer}.",
                movie.Title, movie.MovieId, detail.Id,
                metadata.PosterPath is not null,
                trailerKey is not null);

            return true;
        }
        catch (OperationCanceledException)
        {
            throw; // propagate cancellation to the loop
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "TmdbSyncService: failed to sync '{Title}' (MovieId={MovieId}). " +
                "Will continue with remaining movies.",
                movie.Title, movie.MovieId);
            return false;
        }
    }

    /// <summary>
    /// Replaces all MovieGenre rows for the given movie with the genre list
    /// returned by TMDb. Resolves each TMDb genre ID against the local Genres table.
    /// Genres not yet in the local table are skipped with a warning — run
    /// SyncGenresAsync first to ensure the master list is populated.
    /// </summary>
    private async Task SyncMovieGenresAsync(
        int                                               movieId,
        IEnumerable<TMDbLib.Objects.General.Genre>?       tmdbGenres,
        CancellationToken                                 cancellationToken)
    {
        // Remove existing associations for this movie
        var existing = await _db.MovieGenres
            .Where(mg => mg.MovieId == movieId)
            .ToListAsync(cancellationToken);

        _db.MovieGenres.RemoveRange(existing);

        if (tmdbGenres == null) return;

        var genreList = tmdbGenres.ToList();
        bool isPrimary = true; // first genre in the TMDb list is marked primary

        foreach (var tmdbGenre in genreList)
        {
            // Resolve to local Genre row by TMDbGenreId
            var localGenre = await _db.Genres
                .FirstOrDefaultAsync(g => g.TMDbGenreId == tmdbGenre.Id, cancellationToken);

            if (localGenre is null)
            {
                _logger.LogWarning(
                    "TmdbSyncService: TMDb genre '{Name}' (TMDbGenreId={GenreId}) not found in " +
                    "local Genres table for MovieId={MovieId}. Run SyncGenresAsync first.",
                    tmdbGenre.Name, tmdbGenre.Id, movieId);
                continue;
            }

            _db.MovieGenres.Add(new MovieGenre
            {
                MovieId        = movieId,
                GenreId        = localGenre.GenreId,
                IsPrimaryGenre = isPrimary,
                CreatedUtc     = DateTime.UtcNow
            });

            isPrimary = false; // only the first genre is primary
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a genre name to a URL-safe lowercase slug.
    /// Examples: "Science Fiction" → "science-fiction", "Action" → "action".
    /// Accepts null and returns a safe fallback.
    /// </summary>
    private static string GenerateSlug(string? name)
    {
        var s = (name ?? string.Empty)
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Trim('-');

        return string.IsNullOrWhiteSpace(s) ? "unknown" : s;
    }
}
