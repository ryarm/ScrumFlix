/*
 * File: /ScrumFlix/Models/ConcessionsPricing.cs
 * Description: Represents active pricing for a concession item at a specific theater location.
 */

namespace ScrumFlix.Models;

/// <summary>
/// Location-specific pricing record for a concession inventory item with effective date tracking.
/// </summary>
[Table("concessions_pricing")]
public class ConcessionsPricing
{
    /// <summary>Gets or sets the unique pricing record identifier.</summary>
    [Key]
    [Column("pricing_id")]
    public int PricingId { get; set; }

    /// <summary>Gets or sets the inventory item this pricing applies to.</summary>
    [Column("item_id")]
    public byte[] ItemId { get; set; } = Array.Empty<byte>();

    /// <summary>Gets or sets the location where this price applies.</summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>Gets or sets the selling price for customers.</summary>
    [Column("price")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    /// <summary>Gets or sets when this price became effective.</summary>
    [Column("effective_start")]
    [Display(Name = "Effective From")]
    public DateTime EffectiveStart { get; set; } = DateTime.Now;

    /// <summary>Gets or sets when this price ends (null = currently active).</summary>
    [Column("effective_end")]
    [Display(Name = "Effective Until")]
    public DateTime? EffectiveEnd { get; set; }

    /// <summary>Gets or sets whether this is the current active price.</summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(ItemId))]
    public Inventory? Inventory { get; set; }

    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }
}
