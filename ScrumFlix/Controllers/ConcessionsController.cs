/*
 * File: /ScrumFlix/Controllers/ConcessionsController.cs
 * Description: Controller for the concessions catalog page and adding concession items to the cart.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles the concessions catalog page and cart add operations for food and beverage items.
/// </summary>
public class ConcessionsController : Controller
{
    private readonly AppDbContext _db;
    private readonly CartService _cart;

    /// <summary>
    /// Initializes ConcessionsController with the database context and cart service.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    /// <param name="cart">The session-based cart service.</param>
    public ConcessionsController(AppDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    /// <summary>
    /// Displays the concessions catalog for a given location, showing items and their prices.
    /// Defaults to the first active location if none is specified.
    /// When <paramref name="fromTicketLocationId"/> is supplied the page renders in
    /// "ticket flow" mode: the location is pre-selected to the ticket's theater and a
    /// contextual banner explains why, suppressing the free location switcher.
    /// </summary>
    /// <param name="locationId">The theater location ID to show pricing for.</param>
    /// <param name="fromTicketLocationId">
    /// Set by ShowtimesController when the user clicked "Add Concessions Too".
    /// Forces the catalog to the ticket's location and activates the banner.
    /// </param>
    /// <returns>The concessions catalog view.</returns>
    public async Task<IActionResult> ConcessionsCatalog(int? locationId, int? fromTicketLocationId)
    {
        var locations = await _db.Locations.Where(l => l.IsActive).OrderBy(l => l.LocationName).ToListAsync();

        if (!locations.Any()) return View(new ConcessionsCatalogViewModel());

        // When arriving from the ticket booking flow, pin the location to the ticket's theater.
        var effectiveLocationId = fromTicketLocationId ?? locationId ?? locations.First().LocationId;
        var selectedId = effectiveLocationId;

        // Get distinct item names for this location with active pricing
        var pricingLookup = await _db.ConcessionsPricing
            .Where(cp => cp.LocationId == selectedId && cp.IsActive)
            .Include(cp => cp.Inventory)
            .ToListAsync();

        // Deduplicate by item name (one row per product at this location)
        var seen = new HashSet<string>();
        var items = new List<ConcessionItemDisplayModel>();

        foreach (var pricing in pricingLookup.OrderBy(p => p.Inventory?.ItemName))
        {
            if (pricing.Inventory == null) continue;
            if (!seen.Add(pricing.Inventory.ItemName)) continue;

            items.Add(new ConcessionItemDisplayModel
            {
                Item = pricing.Inventory,
                Price = pricing.Price,
                ItemIdBase64 = Convert.ToBase64String(pricing.Inventory.ItemId)
            });
        }

        // Resolve ticket-location display name for the banner (if in ticket flow)
        string? ticketLocationName = null;
        if (fromTicketLocationId.HasValue)
        {
            ticketLocationName = locations
                .FirstOrDefault(l => l.LocationId == fromTicketLocationId.Value)?.LocationName;
        }

        var vm = new ConcessionsCatalogViewModel
        {
            Items = items,
            Locations = locations,
            SelectedLocationId = selectedId,
            TicketLocationId = fromTicketLocationId,
            TicketLocationName = ticketLocationName
        };

        return View(vm);
    }

    /// <summary>
    /// Displays the location conflict resolution page when the user tries to add a ticket
    /// at a theater that differs from the location of concessions already in their cart.
    /// Reads conflict details from TempData set by ShowtimesController.
    /// </summary>
    /// <returns>The LocationConflict view, or a redirect to the cart if TempData is stale.</returns>
    public IActionResult LocationConflict()
    {
        // If TempData is missing (e.g. user navigated here directly), bail out gracefully.
        if (TempData["ConflictTicketLocationId"] == null)
            return RedirectToAction("CartReview", "Cart");

        ViewBag.TicketLocationId   = TempData.Peek("ConflictTicketLocationId");
        ViewBag.TicketLocationName = TempData.Peek("ConflictTicketLocationName");
        ViewBag.ConcessionLocationId = TempData.Peek("ConflictConcessionLocationId");

        // Resolve the concession location name from the database isn't available here
        // without an async call, so we pass the ID and let the view look it up via
        // a separate label — or we can just show "your current concessions location".
        // TempData entries are preserved (Peek) so RelocateConcessions can still read them.
        return View();
    }

    /// <summary>
    /// Resolves a location conflict. Two outcomes depending on <paramref name="resolution"/>:
    /// <list type="bullet">
    ///   <item><term>switchConcessions</term><description>
    ///     Removes existing concession items from the cart, then re-adds the pending ticket
    ///     and redirects to concessions at the ticket's theater.
    ///   </description></item>
    ///   <item><term>switchMovies</term><description>
    ///     Discards the pending ticket (user will re-select a show at the concessions' theater)
    ///     and redirects to the movie catalog filtered to that location.
    ///   </description></item>
    /// </list>
    /// </summary>
    /// <param name="resolution">"switchConcessions" or "switchMovies".</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RelocateConcessions(string resolution)
    {
        var ticketLocationId   = TempData["ConflictTicketLocationId"]   as int?
                                 ?? (TempData["ConflictTicketLocationId"] is int tli ? tli : (int?)null);
        var concessionLocationId = TempData["ConflictConcessionLocationId"] as int?
                                   ?? (TempData["ConflictConcessionLocationId"] is int cli ? cli : (int?)null);
        var postAction = TempData["PendingTicket_PostAction"] as string;

        if (string.Equals(resolution, "switchConcessions", StringComparison.OrdinalIgnoreCase))
        {
            // Drop all concession items so the user can re-add them at the ticket's theater.
            var cart = _cart.GetCart();
            cart.RemoveAll(c => c.ItemType == CartItemType.Concession);
            // Re-save via a temporary workaround: clear and re-add ticket items.
            // (CartService doesn't expose SaveCart publicly — we rebuild from existing items.)
            _cart.ClearCart();
            foreach (var item in cart) _cart.AddItem(item);

            // Reconstruct and add the pending ticket from TempData.
            var pending = BuildPendingTicketFromTempData();
            if (pending != null) _cart.AddItem(pending);

            TempData["SuccessMessage"] = "Ticket added! Your concessions have been cleared — please re-add them for your theater.";

            // Redirect to concessions pinned to the ticket's theater.
            return RedirectToAction("ConcessionsCatalog", new
            {
                locationId = ticketLocationId,
                fromTicketLocationId = ticketLocationId
            });
        }
        else // switchMovies
        {
            // Discard the pending ticket — user will browse movies at the concessions' theater.
            // We don't add the ticket to the cart; all TempData is consumed/discarded.
            TempData["SuccessMessage"] = "Showing movies at your concessions theater. Select a showtime there.";
            return RedirectToAction("MovieCatalog", "Movies",
                new { locationId = concessionLocationId });
        }
    }

    // ── Private helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Reconstructs a <see cref="CartItem"/> for the pending ticket from TempData entries
    /// written by <c>ShowtimesController.AddTicketToCart</c> before the conflict redirect.
    /// Returns null if any required field is missing.
    /// </summary>
    private CartItem? BuildPendingTicketFromTempData()
    {
        if (TempData["PendingTicket_ShowId"] is not int showId) return null;
        if (TempData["PendingTicket_PriceTierId"] is not int priceTierId) return null;

        decimal unitPrice = TempData["PendingTicket_UnitPrice"] is decimal p ? p : 0m;
        int quantity = TempData["PendingTicket_Quantity"] is int q ? q : 1;
        DateTime? showTime = DateTime.TryParse(TempData["PendingTicket_ShowTime"] as string, out var dt) ? dt : null;

        return new CartItem
        {
            ItemType      = CartItemType.Ticket,
            ShowId        = showId,
            PriceTierId   = priceTierId,
            MovieName     = TempData["PendingTicket_MovieName"] as string,
            ShowTime      = showTime,
            LocationName  = TempData["PendingTicket_LocationName"] as string,
            TierName      = TempData["PendingTicket_TierName"] as string,
            DisplayName   = TempData["PendingTicket_DisplayName"] as string ?? "Ticket",
            UnitPrice     = unitPrice,
            Quantity      = quantity,
            GuestEmail    = TempData["PendingTicket_GuestEmail"] as string
        };
    }

    /// <summary>
    /// Adds a concession item to the shopping cart.
    /// </summary>
    /// <param name="itemIdBase64">Base64-encoded binary item ID.</param>
    /// <param name="itemName">Display name of the item.</param>
    /// <param name="price">Unit price of the item.</param>
    /// <param name="quantity">Quantity to add to cart.</param>
    /// <param name="locationId">Location ID for return redirect.</param>
    /// <returns>Redirects to cart on success or back to concessions on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddConcessionToCart(string itemIdBase64, string itemName, decimal price, int quantity, int locationId)
    {
        if (quantity < 1) quantity = 1;

        var cartItem = new CartItem
        {
            ItemType = CartItemType.Concession,
            InventoryItemId = itemIdBase64,
            DisplayName = itemName,
            UnitPrice = price,
            Quantity = quantity,
            // Persisting the location lets CartService detect cross-location conflicts
            // when the user later tries to add a ticket from a different theater.
            ConcessionLocationId = locationId
        };

        _cart.AddItem(cartItem);

        TempData["SuccessMessage"] = $"{quantity}x {itemName} added to cart!";

        // authoritative count from server
        var cartCount = _cart.GetCart().Sum(ci => ci.Quantity);

        // detect AJAX/fetch requests (X-Requested-With) or explicit JSON accept
        var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                     || (Request.Headers["Accept"].ToString()?.Contains("application/json") == true);

        if (isAjax)
        {
            return Json(new { success = true, cartCount, message = TempData["SuccessMessage"] });
        }

        return RedirectToAction("CartReview", "Cart");
    }
}
