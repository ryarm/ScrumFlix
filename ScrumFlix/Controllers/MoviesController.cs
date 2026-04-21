/*
 * File: /ScrumFlix/Controllers/MoviesController.cs
 * Description: Controller for browsing movies, viewing details, and managing CRUD operations.
 *              Supports keyword title search, cascading Genre→Title dropdown, genre chip pills,
 *              and MPA rating age-restriction awareness via session-stored customer DOB.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles all movie-related requests: catalog browsing, detail view, and admin CRUD actions.
/// </summary>
public class MoviesController : Controller
{
    private readonly AppDbContext _db;

    public MoviesController(AppDbContext db) => _db = db;

    // ── Age helpers ────────────────────────────────────────────────────────────

    /// <summary>
    /// Calculates the viewer's age context from session.
    /// Session key "CustomerDob" (string, ISO date) is written at login.
    /// Returns (isGuest, isUnder17, isUnder18).
    /// </summary>
    private (bool isGuest, bool isUnder17, bool isUnder18) GetViewerAgeContext()
    {
        var dobStr = HttpContext.Session.GetString("CustomerDob");

        if (string.IsNullOrEmpty(dobStr))
            return (isGuest: true, isUnder17: false, isUnder18: false);

        if (!DateTime.TryParse(dobStr, out var dob))
            return (isGuest: false, isUnder17: false, isUnder18: false);

        var today = DateTime.Today;
        int age   = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--; // birthday not yet reached this year

        return (isGuest: false, isUnder17: age < 17, isUnder18: age < 18);
    }

    // ── Catalog ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Displays the movie catalog with optional keyword search and/or genre filter.
    /// Redirects to MovieDetail when a specific movieId is provided (cascade dropdown).
    /// </summary>
    public async Task<IActionResult> MovieCatalog(string? search, string? genre, int? movieId)
    {
        // Cascading dropdown: user picked a specific title → go straight to detail
        if (movieId.HasValue && movieId.Value > 0)
            return RedirectToAction(nameof(MovieDetail), new { id = movieId.Value });

        // Build filtered movie list
        var query = _db.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.MovieName.Contains(search));

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(m => m.Genre == genre);

        // Lookup data for dropdowns and chip pills
        var allMovies = await _db.Movies.OrderBy(m => m.MovieName).ToListAsync();

        var genres = allMovies
            .Select(m => m.Genre)
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        var moviesByGenre = allMovies
            .GroupBy(m => m.Genre)
            .ToDictionary(
                g => g.Key,
                g => g.Select(m => (m.MovieId, m.MovieName)).ToList()
            );

        var (isGuest, isUnder17, isUnder18) = GetViewerAgeContext();

        var vm = new MovieCatalogViewModel
        {
            Movies          = await query.OrderBy(m => m.MovieName).ToListAsync(),
            Genres          = genres,
            MoviesByGenre   = moviesByGenre,
            SearchTerm      = search,
            SelectedGenre   = genre,
            ViewerIsGuest   = isGuest,
            ViewerIsUnder17 = isUnder17,
            ViewerIsUnder18 = isUnder18
        };

        return View(vm);
    }

    // ── Detail ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Displays full details for a single movie including upcoming showtimes.
    /// Includes age-restriction state for the current viewer.
    /// </summary>
    public async Task<IActionResult> MovieDetail(int id)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.MovieId == id);
        if (movie == null) return NotFound();

        var shows = await _db.ScheduledShows
            .Where(ss => ss.MovieId == id && ss.IsActive && ss.ShowDate >= DateTime.Today)
            .Include(ss => ss.Location)
            .Include(ss => ss.TheaterRoom)
            .OrderBy(ss => ss.StartDateTime)
            .ToListAsync();

        var (isGuest, isUnder17, isUnder18) = GetViewerAgeContext();

        var vm = new MovieDetailViewModel
        {
            Movie           = movie,
            UpcomingShows   = shows,
            ViewerIsGuest   = isGuest,
            ViewerIsUnder17 = isUnder17,
            ViewerIsUnder18 = isUnder18
        };

        return View(vm);
    }

    // ── Admin CRUD ─────────────────────────────────────────────────────────────

    public IActionResult MovieCreate() => View(new Movie());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MovieCreate(Movie movie)
    {
        if (!ModelState.IsValid) return View(movie);
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MovieCatalog));
    }

    public async Task<IActionResult> MovieEdit(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        return movie == null ? NotFound() : View(movie);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MovieEdit(int id, Movie movie)
    {
        if (id != movie.MovieId) return BadRequest();
        if (!ModelState.IsValid) return View(movie);
        _db.Movies.Update(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MovieCatalog));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MovieDelete(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie != null) { _db.Movies.Remove(movie); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(MovieCatalog));
    }
}
