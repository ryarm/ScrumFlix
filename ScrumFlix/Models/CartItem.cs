/*
 * File: /ScrumFlix/Models/CartItem.cs
 * Description: Represents an item in the session-based shopping cart (ticket or concession).
 */

namespace ScrumFlix.Models;

/// <summary>
/// Defines the type of item in the cart.
/// </summary>
public enum CartItemType
{
    Ticket,
    Concession
}

/// <summary>
/// A single item held in the user's in-session shopping cart, representing either a ticket or a concession product.
/// </summary>
public class CartItem
{
    /// <summary>Gets or sets a unique identifier for this cart line item.</summary>
    public string CartItemId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>Gets or sets the type of this cart item (Ticket or Concession).</summary>
    public CartItemType ItemType { get; set; }

    // ── Ticket fields ──────────────────────────────────────────────────────────

    /// <summary>Gets or sets the show ID when this is a ticket item.</summary>
    public int? ShowId { get; set; }

    /// <summary>Gets or sets the price tier ID when this is a ticket item.</summary>
    public int? PriceTierId { get; set; }

    /// <summary>Gets or sets the movie name for display on ticket items.</summary>
    public string? MovieName { get; set; }

    /// <summary>Gets or sets the showtime for display on ticket items.</summary>
    public DateTime? ShowTime { get; set; }

    /// <summary>Gets or sets the location name for display on ticket items.</summary>
    public string? LocationName { get; set; }

    /// <summary>Gets or sets the price tier category name for display.</summary>
    public string? TierName { get; set; }

    // ── Concession fields ──────────────────────────────────────────────────────

    /// <summary>Gets or sets the inventory item ID (base64 of byte[]) when this is a concession.</summary>
    public string? InventoryItemId { get; set; }

    // ── Shared fields ──────────────────────────────────────────────────────────

    /// <summary>Gets or sets the display name of this cart item.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the unit price for this item.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Gets or sets the quantity of this item in the cart.</summary>
    public int Quantity { get; set; } = 1;

    /// <summary>Gets or sets an optional guest email for ticket confirmation (ticket items only).</summary>
    public string? GuestEmail { get; set; }

    /// <summary>Gets the total price for this line item.</summary>
    public decimal LineTotal => UnitPrice * Quantity;

    /// <summary>
    /// For concession items only: the LocationId of the theater this concession belongs to.
    /// Populated by ConcessionsController from the locationId form field.
    /// Used by CartService.GetConcessionLocationId() to detect cross-location conflicts.
    /// </summary>
    public int? ConcessionLocationId { get; set; }

}
