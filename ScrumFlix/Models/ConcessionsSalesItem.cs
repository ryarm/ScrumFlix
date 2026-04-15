/*
 * File: /ScrumFlix/Models/ConcessionsSalesItem.cs
 * Description: Represents a line item within a concessions sale transaction.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A single line item in a concessions sale, linking a sale to an inventory item with quantity and price.
/// </summary>
[Table("concessions_sales_items")]
public class ConcessionsSalesItem
{
    /// <summary>Gets or sets the unique sale item identifier.</summary>
    [Key]
    [Column("sale_item_id")]
    public int SaleItemId { get; set; }

    /// <summary>Gets or sets the parent sale this item belongs to.</summary>
    [Column("sale_id")]
    public int SaleId { get; set; }

    /// <summary>Gets or sets the inventory item sold.</summary>
    [Column("item_id")]
    public byte[] ItemId { get; set; } = Array.Empty<byte>();

    /// <summary>Gets or sets the quantity purchased.</summary>
    [Column("quantity")]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    /// <summary>Gets or sets the unit price at time of sale.</summary>
    [Column("unit_price")]
    [DataType(DataType.Currency)]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }

    /// <summary>Gets the computed line total (quantity * unit_price). Stored in DB.</summary>
    [Column("line_total")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public decimal LineTotal { get; set; }

    // Navigation properties
    [ForeignKey(nameof(SaleId))]
    public ConcessionsSale? ConcessionsSale { get; set; }

    [ForeignKey(nameof(ItemId))]
    public Inventory? Inventory { get; set; }
}
