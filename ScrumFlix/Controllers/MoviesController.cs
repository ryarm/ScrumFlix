/*
 * File: /ScrumFlix/Controllers/MoviesController.cs
 * Description: Controller for browsing movies, viewing details, and managing CRUD operations.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles all movie-related requests: catalog browsing, detail view, and admin CRUD actions.
/// </summary>
public class MoviesController : Controller
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes MoviesController with the application database context.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    public MoviesController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Displays the movie catalog with optional search term and genre filter.
    /// </summary>
    /// <param name="search">Optional search term to filter movies by name.</param>
    /// <param name="genre">Optional genre filter.</param>
    /// <returns>The movie catalog view with filtered results.</returns>
    public async Task<IActionResult> MovieCatalog(string? search, string? genre)
    {
        var query = _db.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.MovieName.Contains(search));

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(m => m.Genre == genre);

        var genres = await _db.Movies.Select(m => m.Genre).Distinct().OrderBy(g => g).ToListAsync();

        var vm = new MovieCatalogViewModel
        {
            Movies       = await query.OrderBy(m => m.MovieName).ToListAsync(),
            Genres       = genres,
            SearchTerm   = search,
            SelectedGenre = genre
        };

        return View(vm);
    }

    /// <summary>
    /// Displays full details for a single movie including upcoming showtimes.
    /// </summary>
    /// <param name="id">The movie ID.</param>
    /// <returns>Movie detail view or 404 if not found.</returns>
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

        var vm = new MovieDetailViewModel
        {
            Movie        = movie,
            UpcomingShows = shows
        };

        return View(vm);
    }

    // ── Admin CRUD ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Renders the Create Movie form.
    /// </summary>
    /// <returns>The create movie form view.</returns>
    public IActionResult MovieCreate() => View(new Movie());

    /// <summary>
    /// Handles POST submission to create a new movie.
    /// </summary>
    /// <param name="movie">The movie data submitted from the form.</param>
    /// <returns>Redirects to catalog on success; re-renders form on validation failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MovieCreate(Movie movie)
    {
        if (!ModelState.IsValid) return View(movie);
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MovieCatalog));
    }

    /// <summary>
    /// Renders the Edit Movie form for an existing movie.
    /// </summary>
    /// <param name="id">The movie ID to edit.</param>
    /// <returns>Edit form view or 404 if not found.</returns>
    public async Task<IActionResult> MovieEdit(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        return movie == null ? NotFound() : View(movie);
    }

    /// <summary>
    /// Handles POST submission to update an existing movie.
    /// </summary>
    /// <param name="id">The movie ID being updated.</param>
    /// <param name="movie">The updated movie data from the form.</param>
    /// <returns>Redirects to catalog on success; re-renders form on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MovieEdit(int id, Movie movie)
    {
        if (id != movie.MovieId) return BadRequest();
        if (!ModelState.IsValid) return View(movie);

        _db.Movies.Update(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MovieCatalog));
    }

    /// <summary>
    /// Handles POST deletion of a movie by ID.
    /// </summary>
    /// <param name="id">The movie ID to delete.</param>
    /// <returns>Redirects to catalog after deletion.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MovieDelete(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie != null)
        {
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(MovieCatalog));
    }
}
