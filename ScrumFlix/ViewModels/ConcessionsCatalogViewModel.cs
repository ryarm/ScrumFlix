/*
 * File: /ScrumFlix/ViewModels/ConcessionsCatalogViewModel.cs
 * Description: ViewModel for the concessions browsing page with items and location-based pricing.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// Combines an inventory item with its active location-specific price for display on the concessions page.
/// </summary>
public class ConcessionItemDisplayModel
{
    /// <summary>Gets or sets the inventory item.</summary>
    public Inventory Item { get; set; } = null!;

    /// <summary>Gets or sets the active selling price for this item at the selected location.</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the base64-encoded item ID for use in form values.</summary>
    public string ItemIdBase64 { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for the concessions catalog page listing all available items and a location selector.
/// </summary>
public class ConcessionsCatalogViewModel
{
    /// <summary>Gets or sets the list of concession items available for purchase.</summary>
    public List<ConcessionItemDisplayModel> Items { get; set; } = new();

    /// <summary>Gets or sets all active theater locations for the location selector.</summary>
    public List<Location> Locations { get; set; } = new();

    /// <summary>Gets or sets the currently selected location ID.</summary>
    public int SelectedLocationId { get; set; }

    /// <summary>
    /// When the user arrives here via "Add Concessions Too" from the booking page,
    /// this is the location ID of the ticket they just added. The view uses this to
    /// render a contextual "Showing concessions for your theater" banner and suppress
    /// the free location-switcher so concessions stay pinned to the ticket's location.
    /// Null when the user arrives at the page independently.
    /// </summary>
    public int? TicketLocationId { get; set; }

    /// <summary>Display name of the ticket's theater, shown in the contextual banner.</summary>
    public string? TicketLocationName { get; set; }

    /// <summary>True when the page was reached via the ticket booking flow.</summary>
    public bool IsTicketFlow => TicketLocationId.HasValue;
}
