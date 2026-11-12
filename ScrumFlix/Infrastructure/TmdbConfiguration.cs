/*
 * File: /ScrumFlix/Infrastructure/TmdbConfiguration.cs
 * Namespace: ScrumFlix.Infrastructure
 * Purpose: Registers the scoped TMDb sync service and a separately configured,
 *          currently unused named HttpClient for a possible future transport change.
 *
 * CURRENT PATH (November 2026):
 *   TmdbSyncService constructs TMDbLib 4.0.0's TMDbClient with the API key.
 *   No code obtains IHttpClientFactory.CreateClient("TmdbClient") or passes
 *   this named client to TMDbLib. Its retry, breaker, and timeout settings
 *   therefore do not govern the active sync requests. Do not rely on them
 *   when diagnosing an actual sync failure.
 *
 * HISTORY:
 *   The June 12 sync investigation already identified this unused registration.
 *   Its proposed direct-HTTP fallback and thread-starvation diagnosis were
 *   investigations, not the current implementation or confirmed root cause.
 *   The later background-queue/ProgressHub work is separate from HTTP transport.
 *
 * FUTURE REVIEW:
 *   If failures justify a transport change, first confirm a supported TMDbLib
 *   integration point or design a focused HTTP adapter, then test behavior.
 *   Upgrading a package alone does not connect this named client to the sync.
 *
 * USER SECRETS KEY:
 *   Development:  dotnet user-secrets set "Tmdb:ApiKey" "your_v3_api_key"
 *   Production:   Environment variable Tmdb__ApiKey (double-underscore)
 *   TMDb v3 API keys are obtained at: https://www.themoviedb.org/settings/api
 *
 * IMAGE DELIVERY:
 *   Poster/backdrop URLs currently use the application's /tmdb/* image proxy.
 *   Review SecurityHeadersConfiguration.cs if direct third-party images are added.
 */

using Microsoft.Extensions.Http.Resilience;

namespace ScrumFlix.Infrastructure;

/// <summary>
/// Registers TMDb API client infrastructure and TmdbSyncService with the DI container.
/// Call <see cref="ConfigureTmdb"/> from Program.cs in the service registration block.
/// </summary>
public static class TmdbConfiguration
{
    /// <summary>
    /// Adds the named TMDb HttpClient (with resilience handler) and registers
    /// TmdbSyncService as a scoped service.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configuration">The application configuration (reads Tmdb:ApiKey).</param>
    public static IServiceCollection ConfigureTmdb(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Named HttpClient for TMDb API ──────────────────────────────────────
        //
        // Configuration applies only to callers that request this named client.
        // No TMDb sync caller currently does. The configured limits are 30s total,
        // 10s per attempt, three retries after the initial attempt, and a breaker
        // based on a 50% failure ratio with at least five sampled requests.
        services.AddHttpClient("TmdbClient", client =>
        {
            client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "ScrumFlix/2.0");
        })
        .AddStandardResilienceHandler(options =>
        {
            // Total timeout across all retry attempts
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);

            // Per-attempt timeout (must be less than TotalRequestTimeout)
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);

            // Retry: 3 additional attempts after the initial failure
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.UseJitter        = true; // avoids thundering herd on retry

            // Circuit breaker: pause after repeated failures to avoid hammering TMDb
            options.CircuitBreaker.SamplingDuration           = TimeSpan.FromSeconds(60);
            options.CircuitBreaker.FailureRatio               = 0.5;   // 50% failure rate
            options.CircuitBreaker.MinimumThroughput          = 5;     // min requests before breaking
            options.CircuitBreaker.BreakDuration              = TimeSpan.FromSeconds(30);
        });

        // ── TmdbSyncService ────────────────────────────────────────────────────
        //
        // Scoped: the service creates its TMDbClient alongside its scoped
        // AppDbContext for the request or queued work item's DI scope.
        services.AddScoped<ITmdbSyncService, TmdbSyncService>();

        // Log API key presence at startup (value is never logged — only presence)
        var hasKey = !string.IsNullOrWhiteSpace(configuration["Tmdb:ApiKey"]);
        if (!hasKey)
        {
            // Warning is also emitted by TmdbSyncService itself on first use,
            // but surface it here too so it appears at startup rather than on
            // first sync request.
            Console.WriteLine(
                "[TmdbConfiguration] WARNING: Tmdb:ApiKey is not set. " +
                "Movie poster and genre syncing will be disabled. " +
                "Set the key via: dotnet user-secrets set \"Tmdb:ApiKey\" \"<your_key>\"");
        }

        return services;
    }
}
