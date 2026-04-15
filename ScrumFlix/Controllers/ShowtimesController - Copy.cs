/*
 * File: /ScrumFlix/Controllers/ShowtimesController.cs
 * Description: Controller for viewing showtime details, seat selection, and adding tickets to cart.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles showtime detail viewing, seat selection UI, and ticket cart additions.
/// </summary>
public class ShowtimesController : Controller
{
    private readonly AppDbContext _db;
    private readonly CartService  _cart;

    /// <summary>
    /// Initializes ShowtimesController with the database context and cart service.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    /// <param name="cart">The session-based cart service.</param>
    public ShowtimesController(AppDbContext db, CartService cart)
    {
        _db   = db;
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
            Show       = show,
            PriceTiers = priceTiers,
            Quantity   = 1
        };

        return View(vm);
    }

    /// <summary>
    /// Handles ticket addition to the cart from the showtime booking form.
    /// </summary>
    /// <param name="vm">The booking form data including show ID, price tier, quantity, and guest email.</param>
    /// <returns>Redirects to cart on success; re-renders booking page on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTicketToCart(ShowtimeBookingViewModel vm)
    {
        var show = await _db.ScheduledShows
            .Include(ss => ss.Movie)
            .Include(ss => ss.Location)
            .Include(ss => ss.TheaterRoom)
            .FirstOrDefaultAsync(ss => ss.ShowId == vm.Show.ShowId);

        var priceTier = await _db.PriceTiers.FindAsync(vm.SelectedPriceTierId);

        if (show == null || priceTier == null)
        {
            ModelState.AddModelError("", "Invalid show or price tier selected.");
            vm.Show       = show ?? new ScheduledShow();
            vm.PriceTiers = await _db.PriceTiers.Where(pt => pt.IsActive).ToListAsync();
            return View("ShowtimeBooking", vm);
        }

        if (!ModelState.IsValid)
        {
            vm.Show       = show;
            vm.PriceTiers = await _db.PriceTiers.Where(pt => pt.IsActive).ToListAsync();
            return View("ShowtimeBooking", vm);
        }

        var cartItem = new CartItem
        {
            ItemType      = CartItemType.Ticket,
            ShowId        = show.ShowId,
            PriceTierId   = priceTier.PriceTierId,
            MovieName     = show.Movie?.MovieName,
            ShowTime      = show.StartDateTime,
            LocationName  = show.Location?.LocationName,
            TierName      = priceTier.CategoryName,
            DisplayName   = $"{show.Movie?.MovieName} — {priceTier.CategoryName}",
            UnitPrice     = priceTier.Price,
            Quantity      = vm.Quantity,
            GuestEmail    = vm.GuestEmail
        };

        _cart.AddItem(cartItem);

        TempData["SuccessMessage"] = $"{vm.Quantity} ticket(s) added to cart!";
        return RedirectToAction("CartReview", "Cart");
    }
}
