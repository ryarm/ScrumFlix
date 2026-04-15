/*
 * File: /ScrumFlix/Controllers/CartController.cs
 * Description: Controller for the shopping cart review page, quantity management, and checkout.
 */

namespace ScrumFlix.Controllers;

/// <summary>
/// Handles cart review, item removal, quantity updates, and checkout confirmation.
/// </summary>
public class CartController : Controller
{
    private readonly CartService _cart;
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes CartController with the cart service and database context.
    /// </summary>
    /// <param name="cart">The session-based cart service.</param>
    /// <param name="db">The EF Core database context.</param>
    public CartController(CartService cart, AppDbContext db)
    {
        _cart = cart;
        _db = db;
    }

    /// <summary>
    /// Displays the cart review page with all items, subtotal, tax, and total.
    /// </summary>
    /// <returns>The cart review view.</returns>
    public IActionResult CartReview()
    {
        var vm = new CartViewModel
        {
            Items = _cart.GetCart(),
            Subtotal = _cart.GetSubtotal(),
            Tax = _cart.GetTax(),
            Total = _cart.GetTotal()
        };
        return View(vm);
    }

    /// <summary>
    /// Removes a specific item from the cart by its line item ID.
    /// </summary>
    /// <param name="cartItemId">The unique cart item ID to remove.</param>
    /// <returns>Redirects back to cart review.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveItem(string cartItemId)
    {
        _cart.RemoveItem(cartItemId);
        return RedirectToAction(nameof(CartReview));
    }

    /// <summary>
    /// Updates the quantity of a specific cart line item.
    /// </summary>
    /// <param name="cartItemId">The cart item to update.</param>
    /// <param name="quantity">The new quantity (0 removes the item).</param>
    /// <returns>Redirects back to cart review.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(string cartItemId, int quantity)
    {
        _cart.UpdateQuantity(cartItemId, quantity);
        return RedirectToAction(nameof(CartReview));
    }

    /// <summary>
    /// Clears all items from the cart.
    /// </summary>
    /// <returns>Redirects to cart review.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearCart()
    {
        _cart.ClearCart();
        return RedirectToAction(nameof(CartReview));
    }

    /// <summary>
    /// Processes checkout: saves ticket and concession sale records to the database and clears cart.
    /// </summary>
    /// <returns>Redirects to order confirmation page on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var items = _cart.GetCart();
        if (!items.Any()) return RedirectToAction(nameof(CartReview));

        var defaultLocationId = await _db.Locations
            .Where(l => l.IsActive)
            .Select(l => l.LocationId)
            .FirstOrDefaultAsync();

        // determine current customer id (example: find by user email claim)
        int? currentCustomerId = null;
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var customer = await _db.Customers
                    .Where(c => c.CustomerEmail == userEmail)
                    .Select(c => new { c.CustomerId })
                    .FirstOrDefaultAsync();
                if (customer != null) currentCustomerId = customer.CustomerId;
            }
        }

        // preload price map for ticket items in cart so we can record the purchase price
        var ticketItems = items.Where(i => i.ItemType == CartItemType.Ticket).ToList();
        var priceTierIds = ticketItems
                            .Where(i => i.PriceTierId.HasValue)
                            .Select(i => i.PriceTierId!.Value)
                            .Distinct()
                            .ToList();
        var priceMap = new Dictionary<int, decimal>();
        if (priceTierIds.Any())
        {
            priceMap = await _db.PriceTiers
                                .Where(pt => priceTierIds.Contains(pt.PriceTierId))
                                .ToDictionaryAsync(pt => pt.PriceTierId, pt => pt.Price);
        }

        // ── Save tickets (with server-side availability guard and transaction) ──
        var ticketItemsList = items.Where(i => i.ItemType == CartItemType.Ticket).ToList();
        if (ticketItemsList.Any())
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in ticketItemsList)
                {
                    // ensure required nullable fields are present
                    if (!item.ShowId.HasValue || !item.PriceTierId.HasValue)
                        return BadRequest("Cart contains an invalid ticket (missing show or price tier).");

                    // validation for guest purchases
                    if (!currentCustomerId.HasValue && string.IsNullOrWhiteSpace(item.GuestEmail))
                        return BadRequest("Guest email required for guest ticket purchases.");

                    // load show with room to compute availability
                    var show = await _db.ScheduledShows
                                         .Include(s => s.TheaterRoom)
                                         .FirstOrDefaultAsync(s => s.ShowId == item.ShowId.Value);
                    if (show == null) return NotFound("Show not found.");

                    var remaining = (show.TheaterRoom?.SeatingCapacity ?? 0) - show.TicketsSold;
                    if (remaining < item.Quantity)
                        return BadRequest($"Not enough seats available for show {show.ShowId}. Requested: {item.Quantity}, Remaining: {remaining}");

                    for (int q = 0; q < item.Quantity; q++)
                    {
                        var ticket = new Ticket
                        {
                            ShowId = item.ShowId.Value,
                            PriceTierId = item.PriceTierId.Value,
                            PriceAtPurchase = priceMap.TryGetValue(item.PriceTierId.Value, out var p) ? p : 0m,
                            CustomerId = currentCustomerId,
                            GuestEmail = currentCustomerId.HasValue ? null : item.GuestEmail,
                            TicketDate = DateTime.Today
                        };
                        _db.Tickets.Add(ticket);
                    }

                    // increment tickets sold by quantity
                    show.TicketsSold += item.Quantity;
                    _db.ScheduledShows.Update(show);
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // ── Save concessions sale ──────────────────────────────────────────────
        var concessionItems = items.Where(i => i.ItemType == CartItemType.Concession).ToList();
        if (concessionItems.Any())
        {
            var sale = new ConcessionsSale
            {
                LocationId = defaultLocationId,
                SaleDatetime = DateTime.Now,
                TotalAmount = concessionItems.Sum(i => i.LineTotal)
            };
            _db.ConcessionsSales.Add(sale);
            await _db.SaveChangesAsync();

            foreach (var ci in concessionItems)
            {
                if (string.IsNullOrEmpty(ci.InventoryItemId)) continue;
                var saleItem = new ConcessionsSalesItem
                {
                    SaleId = sale.SaleId,
                    ItemId = Convert.FromBase64String(ci.InventoryItemId),
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                };
                _db.ConcessionsSalesItems.Add(saleItem);
            }
        }

        await _db.SaveChangesAsync();

        var total = _cart.GetTotal();
        _cart.ClearCart();

        TempData["OrderTotal"] = total.ToString("C");
        TempData["SuccessMessage"] = "Your order has been placed successfully!";

        return RedirectToAction(nameof(OrderConfirmation));
    }

    /// <summary>
    /// Returns the current cart item count as JSON for live badge updates.
    /// Called via fetch() from the layout's refreshCartBadge() helper.
    /// </summary>
    /// <returns>JSON object: { count: int }</returns>
    [HttpGet]
    public IActionResult GetCartCount()
    {
        return Json(new { count = _cart.GetItemCount() });
    }

    /// <summary>
    /// Displays the order confirmation page after a successful checkout.
    /// </summary>
    /// <returns>The order confirmation view.</returns>
    public IActionResult OrderConfirmation()
    {
        ViewBag.OrderTotal = TempData["OrderTotal"];
        return View();
    }
}
