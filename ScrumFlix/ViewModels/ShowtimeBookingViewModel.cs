/*
 * File: /ScrumFlix/ViewModels/ShowtimeBookingViewModel.cs
 * Description: ViewModel for the showtime booking/seat selection page. Includes required email
 *              and confirm-email fields with full validation for ticket confirmation.
 */

namespace ScrumFlix.ViewModels;

/// <summary>
/// ViewModel for the showtime booking page, presenting show details, seating info, price tiers,
/// and a required email address (with confirmation) for ticket receipt.
/// </summary>
public class ShowtimeBookingViewModel
{
    /// <summary>Gets or sets the scheduled show being booked.</summary>
    public ScheduledShow Show { get; set; } = null!;

    /// <summary>Gets or sets the available price tiers the customer can select.</summary>
    public List<PriceTier> PriceTiers { get; set; } = new();

    /// <summary>Gets or sets the selected price tier ID from the form.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Please select a ticket type.")]
    public int SelectedPriceTierId { get; set; }

    /// <summary>Gets or sets the quantity of tickets requested.</summary>
    [Range(1, 20, ErrorMessage = "You can purchase between 1 and 20 tickets.")]
    public int Quantity { get; set; } = 1;

    /// <summary>Gets or sets the email address used for ticket confirmation. Required.</summary>
    [Required(ErrorMessage = "An email address is required for your ticket confirmation.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(255, ErrorMessage = "Email address must be 255 characters or fewer.")]
    [Display(Name = "Email Address")]
    public string GuestEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the confirmation email — must match GuestEmail exactly.</summary>
    [Required(ErrorMessage = "Please confirm your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Compare(nameof(GuestEmail), ErrorMessage = "Email addresses do not match. Please check and try again.")]
    [Display(Name = "Confirm Email Address")]
    public string ConfirmEmail { get; set; } = string.Empty;
}
