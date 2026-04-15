/*
 * File: /ScrumFlix/ViewModels/MovieDetailViewModel.cs
 * Description: ViewModel for the movie detail page showing metadata and available showtimes.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// ViewModel for the movie detail page, including movie metadata and associated showtimes.
/// </summary>
public class MovieDetailViewModel
{
    /// <summary>Gets or sets the movie being displayed.</summary>
    public Movie Movie { get; set; } = null!;

    /// <summary>Gets or sets the upcoming scheduled shows for this movie, grouped by date.</summary>
    public List<ScheduledShow> UpcomingShows { get; set; } = new();
}
