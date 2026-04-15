/*
 * File: /ScrumFlix/Models/PriceTier.cs
 * Description: Represents a ticket price tier (e.g., Adult Standard, Child, Senior, IMAX, Matinee).
 */

namespace ScrumFlix.Models;

/// <summary>
/// A ticket pricing tier defining category, format, and price for ticket purchases.
/// </summary>
[Table("price_tiers")]
public class PriceTier
{
    /// <summary>Gets or sets the unique price tier identifier.</summary>
    [Key]
    [Column("price_tier_id")]
    public int PriceTierId { get; set; }

    /// <summary>Gets or sets the display name for this tier (e.g., "Adult Standard").</summary>
    [Required]
    [MaxLength(50)]
    [Column("category_name")]
    [Display(Name = "Category")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Gets or sets the format type (e.g., "2D", "IMAX", "PLF").</summary>
    [MaxLength(20)]
    [Column("format_type")]
    [Display(Name = "Format")]
    public string FormatType { get; set; } = "2D";

    /// <summary>Gets or sets the ticket price for this tier.</summary>
    [Column("price")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    /// <summary>Gets or sets whether this price tier is currently active.</summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the description shown to customers.</summary>
    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
