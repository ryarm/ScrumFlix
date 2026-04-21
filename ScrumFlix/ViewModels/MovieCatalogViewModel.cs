/*
 * File: /ScrumFlix/ViewModels/MovieCatalogViewModel.cs
 * Description: ViewModel for the movie catalog page. Supports keyword title search,
 *              cascading Genre → Movie Title dropdowns, genre chip pill filtering,
 *              and age-restriction awareness for R and NC-17 rated movies.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// ViewModel for rendering the movie catalog page with title search,
/// cascading genre/title dropdowns, genre chip pill filters,
/// and MPA rating visibility rules based on the viewer's age.
/// </summary>
public class MovieCatalogViewModel
{
    /// <summary>Gets or sets the list of movies matching the current filter.</summary>
    public List<Movie> Movies { get; set; } = new();

    /// <summary>Gets or sets all distinct genres for the genre chip pills and Genre dropdown.</summary>
    public List<string> Genres { get; set; } = new();

    /// <summary>
    /// Gets or sets movies grouped by genre for the cascading dropdown.
    /// Key = genre name, Value = list of (MovieId, MovieName) tuples in that genre.
    /// </summary>
    public Dictionary<string, List<(int MovieId, string MovieName)>> MoviesByGenre { get; set; } = new();

    /// <summary>Gets or sets the current keyword search term (title search bar).</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Gets or sets the active genre filter.</summary>
    public string? SelectedGenre { get; set; }

    /// <summary>
    /// Gets or sets the movie ID chosen in the cascading title dropdown.
    /// When set, the controller redirects directly to MovieDetail.
    /// </summary>
    public int? SelectedMovieId { get; set; }

    // ── Age / Rating Awareness ────────────────────────────────────────────────

    /// <summary>
    /// Gets or sets whether the current viewer is a logged-in customer under 17.
    /// When true: R-rated cards show a warning badge but remain purchasable.
    ///            NC-17 cards are locked and require login for age verification.
    /// </summary>
    public bool ViewerIsUnder17 { get; set; }

    /// <summary>
    /// Gets or sets whether the current viewer is a logged-in customer under 18.
    /// When true: NC-17 cards are hard-blocked regardless of login status.
    /// </summary>
    public bool ViewerIsUnder18 { get; set; }

    /// <summary>
    /// Gets or sets whether the current viewer is a guest (not logged in).
    /// Guests: R → warning badge only. NC-17 → login required prompt.
    /// </summary>
    public bool ViewerIsGuest { get; set; }

    /// <summary>
    /// Determines the age-restriction state for a given MPA rating.
    /// Returns: "none" | "r-warning" | "nc17-login-required" | "nc17-hard-blocked"
    /// </summary>
    public string GetRatingState(string mpaRating) => mpaRating?.ToUpper() switch
    {
        "R" when ViewerIsUnder17  => "r-warning",       // Under 17 logged in → warning
        "R" when ViewerIsGuest    => "r-warning",       // Guest → warning (can still purchase)
        "NC-17" when ViewerIsGuest => "nc17-login-required",  // Guest → must log in
        "NC-17" when ViewerIsUnder18 => "nc17-hard-blocked",  // Under 18 logged in → hard block
        _ => "none"
    };
}
