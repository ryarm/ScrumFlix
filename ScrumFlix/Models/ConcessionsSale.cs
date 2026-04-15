/*
 * File: /ScrumFlix/Models/ConcessionsSale.cs
 * Description: Represents a completed concessions sale transaction at a theater location.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A completed concessions sale transaction, linking a location, employee, and total amount.
/// </summary>
[Table("concessions_sales")]
public class ConcessionsSale
{
    /// <summary>Gets or sets the unique sale identifier.</summary>
    [Key]
    [Column("sale_id")]
    public int SaleId { get; set; }

    /// <summary>Gets or sets the location where the sale occurred.</summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>Gets or sets the date and time of the sale.</summary>
    [Column("sale_datetime")]
    [Display(Name = "Sale Date/Time")]
    public DateTime SaleDatetime { get; set; } = DateTime.Now;

    /// <summary>Gets or sets the optional employee who processed the sale.</summary>
    [Column("employee_id")]
    public int? EmployeeId { get; set; }

    /// <summary>Gets or sets the total amount of the sale.</summary>
    [Column("total_amount")]
    [DataType(DataType.Currency)]
    [Display(Name = "Total")]
    public decimal TotalAmount { get; set; }

    // Navigation properties
    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }

    public ICollection<ConcessionsSalesItem> SalesItems { get; set; } = new List<ConcessionsSalesItem>();
}
