/*
 * File: /ScrumFlix/ViewModels/MovieDetailViewModel.cs
 * Description: ViewModel for the movie detail page showing metadata, showtimes,
 *              and age-restriction state for the current viewer.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// ViewModel for the movie detail page, including movie metadata, upcoming showtimes,
/// and the MPA rating restriction state for the current viewer.
/// </summary>
public class MovieDetailViewModel
{
    /// <summary>Gets or sets the movie being displayed.</summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>Gets or sets the upcoming scheduled shows for this movie.</summary>
    public List<ScheduledShow> UpcomingShows { get; set; } = new();

    // ── Age / Rating Awareness ────────────────────────────────────────────────

    /// <summary>Whether the viewer is a guest (not logged in).</summary>
    public bool ViewerIsGuest { get; set; }

    /// <summary>Whether the logged-in viewer is under 17.</summary>
    public bool ViewerIsUnder17 { get; set; }

    /// <summary>Whether the logged-in viewer is under 18.</summary>
    public bool ViewerIsUnder18 { get; set; }

    /// <summary>
    /// Returns the age-restriction state for this movie's rating:
    /// "none" | "r-warning" | "nc17-login-required" | "nc17-hard-blocked"
    /// </summary>
    public string RatingState => Movie?.MpaRating?.ToUpper() switch
    {
        "R" when ViewerIsUnder17  => "r-warning",
        "R" when ViewerIsGuest    => "r-warning",
        "NC-17" when ViewerIsGuest   => "nc17-login-required",
        "NC-17" when ViewerIsUnder18 => "nc17-hard-blocked",
        _ => "none"
    };
}
