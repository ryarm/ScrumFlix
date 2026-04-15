/*
 * File: /ScrumFlix/ViewModels/ShowtimeBookingViewModel.cs
 * Description: ViewModel for the showtime booking/seat selection page.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// ViewModel for the showtime booking page, presenting show details, seating info, and price tiers.
/// </summary>
public class ShowtimeBookingViewModel
{
    /// <summary>Gets or sets the scheduled show being booked.</summary>
    public ScheduledShow Show { get; set; } = null!;

    /// <summary>Gets or sets the available price tiers the customer can select.</summary>
    public List<PriceTier> PriceTiers { get; set; } = new();

    /// <summary>Gets or sets the selected price tier ID from the form.</summary>
    public int SelectedPriceTierId { get; set; }

    /// <summary>Gets or sets the quantity of tickets requested.</summary>
    [Range(1, 20, ErrorMessage = "You can purchase between 1 and 20 tickets.")]
    public int Quantity { get; set; } = 1;

    /// <summary>Gets or sets the optional guest email for non-registered purchasers.</summary>
    [EmailAddress]
    public string? GuestEmail { get; set; }
}
