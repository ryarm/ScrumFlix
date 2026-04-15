/*
 * File: /ScrumFlix/Data/CartService.cs
 * Description: Service for managing the session-based shopping cart, supporting both ticket
 *              and concession items with quantity management and tax calculation by location.
 */

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ScrumFlix.Data;

/// <summary>
/// Manages a user's shopping cart stored in the ASP.NET Core HTTP session.
/// Supports adding/removing tickets and concessions and calculates location-based sales tax.
/// </summary>
public class CartService
{
    private const string CartKey = "ScrumFlix_Cart";

    // Texas base sales tax rate applied to all transactions
    private const decimal BaseTaxRate = 0.0825m;

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new CartService with the HTTP context accessor for session access.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor for the current HTTP context and session.</param>
    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // ── Session Helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Retrieves the current cart from the session.
    /// </summary>
    /// <returns>A list of CartItems currently in the session cart.</returns>
    public List<CartItem> GetCart()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        var json = session?.GetString(CartKey);
        return string.IsNullOrEmpty(json)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(json) ?? new List<CartItem>();
    }

    /// <summary>
    /// Saves the updated cart back to the session.
    /// </summary>
    /// <param name="cart">The cart list to persist.</param>
    private void SaveCart(List<CartItem> cart)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.SetString(CartKey, JsonConvert.SerializeObject(cart));
    }

    // ── Cart Operations ────────────────────────────────────────────────────────

    /// <summary>
    /// Adds a new item to the cart or increments its quantity if it already exists.
    /// Ticket items are matched by ShowId + PriceTierId; concessions by InventoryItemId.
    /// </summary>
    /// <param name="item">The CartItem to add.</param>
    public void AddItem(CartItem item)
    {
        var cart = GetCart();

        CartItem? existing = item.ItemType == CartItemType.Ticket
            ? cart.FirstOrDefault(c => c.ItemType == CartItemType.Ticket
                                    && c.ShowId == item.ShowId
                                    && c.PriceTierId == item.PriceTierId)
            : cart.FirstOrDefault(c => c.ItemType == CartItemType.Concession
                                    && c.InventoryItemId == item.InventoryItemId);

        if (existing != null)
            existing.Quantity += item.Quantity;
        else
            cart.Add(item);

        SaveCart(cart);
    }

    /// <summary>
    /// Removes a specific line item from the cart by its CartItemId.
    /// </summary>
    /// <param name="cartItemId">The unique identifier of the cart line item to remove.</param>
    public void RemoveItem(string cartItemId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.CartItemId == cartItemId);
        SaveCart(cart);
    }

    /// <summary>
    /// Updates the quantity of a specific cart line item. Removes the item if quantity reaches zero.
    /// </summary>
    /// <param name="cartItemId">The cart item to update.</param>
    /// <param name="quantity">The new quantity value.</param>
    public void UpdateQuantity(string cartItemId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.CartItemId == cartItemId);
        if (item == null) return;

        if (quantity <= 0)
            cart.Remove(item);
        else
            item.Quantity = quantity;

        SaveCart(cart);
    }

    /// <summary>
    /// Clears all items from the cart.
    /// </summary>
    public void ClearCart()
    {
        _httpContextAccessor.HttpContext?.Session.Remove(CartKey);
    }

    // ── Totals & Tax ───────────────────────────────────────────────────────────

    /// <summary>
    /// Calculates the subtotal (pre-tax) of all items in the cart.
    /// </summary>
    /// <returns>Decimal subtotal amount.</returns>
    public decimal GetSubtotal()
    {
        return GetCart().Sum(c => c.LineTotal);
    }

    /// <summary>
    /// Calculates sales tax based on the Texas base rate.
    /// </summary>
    /// <returns>Decimal tax amount.</returns>
    public decimal GetTax()
    {
        return Math.Round(GetSubtotal() * BaseTaxRate, 2);
    }

    /// <summary>
    /// Calculates the grand total including sales tax.
    /// </summary>
    /// <returns>Decimal total amount with tax applied.</returns>
    public decimal GetTotal()
    {
        return GetSubtotal() + GetTax();
    }

    /// <summary>
    /// Returns the total number of individual items (sum of quantities) in the cart.
    /// </summary>
    /// <returns>Integer count of items.</returns>
    public int GetItemCount()
    {
        return GetCart().Sum(c => c.Quantity);
    }

    /// <summary>
    /// Returns the LocationId of the first concession item in the cart, or null if there
    /// are no concession items.  Used by ShowtimesController to detect a cross-location
    /// conflict before adding a ticket at a different theater.
    /// </summary>
    /// <remarks>
    /// Concession items carry their location via the <c>locationId</c> form field, which
    /// is stored on <see cref="CartItem.ConcessionLocationId"/>.  If that property does
    /// not exist yet on CartItem, add it as a nullable int; ConcessionsController already
    /// passes locationId in the Add form so it only needs to be mapped through.
    /// </remarks>
    public int? GetConcessionLocationId()
    {
        return GetCart()
            .Where(c => c.ItemType == CartItemType.Concession && c.ConcessionLocationId.HasValue)
            .Select(c => c.ConcessionLocationId)
            .FirstOrDefault();
    }
}
