/*
 * File: /ScrumFlix/Models/Ticket.cs
 * Description: Represents a purchased movie ticket linked to a scheduled show, price tier, and optional customer.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A ticket purchased for a specific scheduled show, associated with a price tier and optional customer.
/// </summary>
[Table("tickets")]
public class Ticket
{
    /// <summary>Gets or sets the unique ticket identifier.</summary>
    [Key]
    [Column("ticket_id")]
    public int TicketId { get; set; }

    /// <summary>Gets or sets the scheduled show this ticket is for.</summary>
    [Column("show_id")]
    public int ShowId { get; set; }

    /// <summary>Gets or sets the pricing tier applied to this ticket.</summary>
    [Column("price_tier_id")]
    public int PriceTierId { get; set; }

    /// <summary>Gets or sets the optional registered customer who purchased this ticket.</summary>
    [Column("customer_id")]
    public int? CustomerId { get; set; }

    /// <summary>Gets or sets an optional guest email for non-registered purchasers.</summary>
    [MaxLength(255)]
    [EmailAddress]  
    [Column("guest_email")]
    [Display(Name = "Guest Email")]
    public string? GuestEmail { get; set; }

    /// <summary>Gets or sets the date this ticket was issued.</summary>
    [Column("ticket_date")]
    [DataType(DataType.Date)]
    [Display(Name = "Ticket Date")]
    public DateTime TicketDate { get; set; }

    /// <summary>Gets or sets the price at which the ticket was purchased.</summary>
    [Column("price_at_purchase")]
    [Display(Name = "Price at Purchase")]
    public decimal PriceAtPurchase { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ShowId))]
    public ScheduledShow? ScheduledShow { get; set; }

    [ForeignKey(nameof(PriceTierId))]
    public PriceTier? PriceTier { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }
}
