/*
 * File: /ScrumFlix/Controllers/ShowtimesController.cs
 * Description: Controller for viewing showtime details, seat selection, and adding tickets to cart.
 *              Enforces server-side validation of the required guest email before adding to cart.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles showtime detail viewing, seat selection UI, and ticket cart additions.
/// </summary>
public class ShowtimesController : Controller
{
    private readonly AppDbContext _db;
    private readonly CartService _cart;

    /// <summary>
    /// Initializes ShowtimesController with the database context and cart service.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    /// <param name="cart">The session-based cart service.</param>
    public ShowtimesController(AppDbContext db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    /// <summary>
    /// Displays the showtime booking page for a specific scheduled show.
    /// </summary>
    /// <param name="id">The scheduled show ID.</param>
    /// <returns>Showtime booking view or 404 if not found.</returns>
    public async Task<IActionResult> ShowtimeBooking(int id)
    {
        var show = await _db.ScheduledShows
            .Include(ss => ss.Movie)
            .Include(ss => ss.Location)
            .Include(ss => ss.TheaterRoom)
            .FirstOrDefaultAsync(ss => ss.ShowId == id && ss.IsActive);

        if (show == null) return NotFound();

        var priceTiers = await _db.PriceTiers
            .Where(pt => pt.IsActive)
            .OrderBy(pt => pt.Price)
            .ToListAsync();

        var vm = new ShowtimeBookingViewModel
        {
            Show = show,
            PriceTiers = priceTiers,
            Quantity = 1
        };

        return View(vm);
    }

    /// <summary>
    /// Handles ticket addition to the cart. Validates the booking form including the required
    /// guest email and matching confirm-email before adding any items to the cart.
    /// </summary>
    /// <param name="vm">The booking form data including show ID, price tier, quantity, and email.</param>
    /// <returns>Redirects to cart on success; re-renders booking page with errors on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTicketToCart(ShowtimeBookingViewModel vm, string? postAction)
    {
        // Re-load navigation data needed to re-render the form on failure
        var show = await _db.ScheduledShows
            .Include(ss => ss.Movie)
            .Include(ss => ss.Location)
            .Include(ss => ss.TheaterRoom)
            .FirstOrDefaultAsync(ss => ss.ShowId == vm.Show.ShowId);

        var priceTiers = await _db.PriceTiers
            .Where(pt => pt.IsActive)
            .OrderBy(pt => pt.Price)
            .ToListAsync();

        if (show == null)
        {
            ModelState.AddModelError("", "The selected show could not be found.");
            vm.PriceTiers = priceTiers;
            vm.Show = new ScheduledShow();
            return View("ShowtimeBooking", vm);
        }

        // Re-attach navigation properties for view rendering
        vm.Show = show;
        vm.PriceTiers = priceTiers;

        // Bail out if any validation attribute (Required, EmailAddress, Compare, Range) failed
        if (!ModelState.IsValid)
            return View("ShowtimeBooking", vm);

        var priceTier = await _db.PriceTiers.FindAsync(vm.SelectedPriceTierId);
        if (priceTier == null || !priceTier.IsActive)
        {
            ModelState.AddModelError(nameof(vm.SelectedPriceTierId), "The selected ticket type is no longer available.");
            return View("ShowtimeBooking", vm);
        }

        // Guard: enough seats remaining
        if (show.AvailableSeats < vm.Quantity)
        {
            ModelState.AddModelError("", $"Only {show.AvailableSeats} seat(s) remaining for this showing.");
            return View("ShowtimeBooking", vm);
        }

        // Build the cart item — ConfirmEmail is intentionally NOT stored
        var cartItem = new CartItem
        {
            ItemType = CartItemType.Ticket,
            ShowId = show.ShowId,
            PriceTierId = priceTier.PriceTierId,
            MovieName = show.Movie?.MovieName,
            ShowTime = show.StartDateTime,
            LocationName = show.Location?.LocationName,
            TierName = priceTier.CategoryName,
            DisplayName = $"{show.Movie?.MovieName} — {priceTier.CategoryName}",
            UnitPrice = priceTier.Price,
            Quantity = vm.Quantity,
            GuestEmail = vm.GuestEmail
        };

        // ── Scenario B: concessions already in cart at a different location ────
        // Before adding the ticket, check whether the cart already contains concession
        // items locked to a different theater.  If so, redirect to the conflict page
        // instead of silently allowing a cross-location order.
        var existingConcessionLocationId = _cart.GetConcessionLocationId();
        if (existingConcessionLocationId.HasValue
            && existingConcessionLocationId.Value != show.LocationId)
        {
            // Stash the pending ticket details in TempData so the conflict page can
            // offer "switch concessions to this theater" without losing the booking.
            TempData["ConflictTicketShowId"]    = show.ShowId;
            TempData["ConflictTicketLocationId"] = show.LocationId;
            TempData["ConflictTicketLocationName"] = show.Location?.LocationName;
            TempData["ConflictConcessionLocationId"] = existingConcessionLocationId.Value;

            // Also cache the pending cart item so AddTicketToCart can be replayed
            // after the user resolves the conflict.  We serialise via JSON-safe fields.
            TempData["PendingTicket_ShowId"]       = cartItem.ShowId;
            TempData["PendingTicket_PriceTierId"]  = cartItem.PriceTierId;
            TempData["PendingTicket_MovieName"]    = cartItem.MovieName;
            TempData["PendingTicket_ShowTime"]     = cartItem.ShowTime?.ToString("o");
            TempData["PendingTicket_LocationName"] = cartItem.LocationName;
            TempData["PendingTicket_TierName"]     = cartItem.TierName;
            TempData["PendingTicket_DisplayName"]  = cartItem.DisplayName;
            TempData["PendingTicket_UnitPrice"]    = cartItem.UnitPrice;
            TempData["PendingTicket_Quantity"]     = cartItem.Quantity;
            TempData["PendingTicket_GuestEmail"]   = cartItem.GuestEmail;
            TempData["PendingTicket_PostAction"]   = postAction;

            return RedirectToAction("LocationConflict", "Concessions");
        }

        _cart.AddItem(cartItem);

        TempData["SuccessMessage"] = $"{vm.Quantity} ticket(s) added to your cart!";

        // ── Scenario A: ticket added first, now heading to concessions ───────
        // Pass the show's LocationId so ConcessionsCatalog auto-selects the right
        // theater and renders the contextual "Showing concessions for your theater" banner.
        if (string.Equals(postAction, "addAndGoToConcessions", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("ConcessionsCatalog", "Concessions",
                new { locationId = show.LocationId, fromTicketLocationId = show.LocationId });
        }

        // Otherwise return to cart
        return RedirectToAction("CartReview", "Cart");
    }
}
