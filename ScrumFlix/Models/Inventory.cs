/*
 * File: /ScrumFlix/Models/Inventory.cs
 * Description: Represents a concession product in inventory at a specific theater location.
 */

namespace ScrumFlix.Models;

/// <summary>
/// An inventory item (concession product) stocked at a specific theater location from a vendor.
/// </summary>
[Table("inventory")]
public class Inventory
{
    /// <summary>Gets or sets the unique item identifier (UUID binary).</summary>
    [Key]
    [Column("item_id")]
    public byte[] ItemId { get; set; } = Guid.NewGuid().ToByteArray();

    /// <summary>Gets or sets the display name of the concession item.</summary>
    [Required]
    [MaxLength(100)]
    [Column("item_name")]
    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>Gets or sets the unit cost the theater pays for this item.</summary>
    [Column("unit_cost")]
    [DataType(DataType.Currency)]
    [Display(Name = "Unit Cost")]
    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; set; }

    /// <summary>Gets or sets the current stock quantity at this location.</summary>
    [Column("item_quantity")]
    [Display(Name = "Quantity")]
    public int ItemQuantity { get; set; }

    /// <summary>Gets or sets the vendor who supplies this item.</summary>
    [Column("vendor_id")]
    public byte[] VendorId { get; set; } = Array.Empty<byte>();

    /// <summary>Gets or sets the location where this inventory is stocked.</summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(VendorId))]
    public Vendor? Vendor { get; set; }

    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }

    public ICollection<ConcessionsPricing> ConcessionsPricings { get; set; } = new List<ConcessionsPricing>();
    public ICollection<ConcessionsSalesItem> ConcessionsSalesItems { get; set; } = new List<ConcessionsSalesItem>();
    public ICollection<ConcessionsOrder> ConcessionsOrders { get; set; } = new List<ConcessionsOrder>();
}
