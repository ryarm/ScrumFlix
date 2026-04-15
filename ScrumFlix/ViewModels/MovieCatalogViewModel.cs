/*
 * File: /ScrumFlix/ViewModels/MovieCatalogViewModel.cs
 * Description: ViewModel for the movie catalog/browse page with search and genre filtering support.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// ViewModel for rendering the movie catalog page with optional search and genre filters.
/// </summary>
public class MovieCatalogViewModel
{
    /// <summary>Gets or sets the list of movies matching the current filter.</summary>
    public List<Movie> Movies { get; set; } = new();

    /// <summary>Gets or sets the list of available genre options for the filter dropdown.</summary>
    public List<string> Genres { get; set; } = new();

    /// <summary>Gets or sets the current search term entered by the user.</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Gets or sets the currently selected genre filter.</summary>
    public string? SelectedGenre { get; set; }
}
