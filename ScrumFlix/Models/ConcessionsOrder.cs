/*
 * File: /ScrumFlix/Models/ConcessionsOrder.cs
 * Description: Represents a purchase order from a vendor to restock concession inventory at a location.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A vendor purchase order for restocking concession inventory at a specific theater location.
/// </summary>
[Table("concessions_orders")]
public class ConcessionsOrder
{
    /// <summary>Gets or sets the unique order identifier.</summary>
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    /// <summary>Gets or sets the date the order was placed.</summary>
    [Column("order_date")]
    [Display(Name = "Order Date")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    /// <summary>Gets or sets the vendor supplying this order.</summary>
    [Column("vendor_id")]
    public byte[] VendorId { get; set; } = Array.Empty<byte>();

    /// <summary>Gets or sets the inventory item being ordered.</summary>
    [Column("item_id")]
    public byte[] ItemId { get; set; } = Array.Empty<byte>();

    /// <summary>Gets or sets the quantity ordered.</summary>
    [Column("order_quantity")]
    [Range(1, int.MaxValue)]
    [Display(Name = "Quantity")]
    public int OrderQuantity { get; set; }

    /// <summary>Gets or sets the unit price paid to the vendor.</summary>
    [Column("unit_price")]
    [DataType(DataType.Currency)]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }

    /// <summary>Gets or sets the location receiving this order.</summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>Gets or sets whether the order has been received.</summary>
    [Column("order_received")]
    [Display(Name = "Received")]
    public bool OrderReceived { get; set; }

    /// <summary>Gets or sets when the order was received.</summary>
    [Column("order_received_date")]
    [Display(Name = "Received Date")]
    public DateTime? OrderReceivedDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(VendorId))]
    public Vendor? Vendor { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Inventory? Inventory { get; set; }

    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }
}
