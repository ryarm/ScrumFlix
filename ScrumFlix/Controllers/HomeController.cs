/*
 * File: /ScrumFlix/Controllers/HomeController.cs
 * Description: Controller for the home/landing page, featuring now-showing movies and upcoming showtimes.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles requests for the ScrumFlix home dashboard including featured movies and now-showing content.
/// </summary>
public class HomeController : Controller
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes HomeController with the application database context.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Renders the home dashboard with featured and now-showing movies pulled from active showtimes.
    /// </summary>
    /// <returns>The home dashboard view with movie and showtime data.</returns>
    public async Task<IActionResult> HomeDashboard()
    {
        var today = DateTime.Today;

        // Featured: movies with shows today or the next 2 days
        var featuredMovies = await _db.ScheduledShows
            .Where(ss => ss.IsActive && ss.ShowDate >= today && ss.ShowDate <= today.AddDays(2))
            .Include(ss => ss.Movie)
            .Select(ss => ss.Movie!)
            .Distinct()
            .Take(6)
            .ToListAsync();

        // Now Showing: all shows today
        var nowShowing = await _db.ScheduledShows
            .Where(ss => ss.IsActive && ss.ShowDate == today)
            .Include(ss => ss.Movie)
            .Include(ss => ss.Location)
            .Include(ss => ss.TheaterRoom)
            .OrderBy(ss => ss.StartDateTime)
            .Take(8)
            .ToListAsync();

        ViewBag.FeaturedMovies = featuredMovies;
        ViewBag.NowShowing     = nowShowing;

        return View();
    }
}
