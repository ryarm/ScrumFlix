/*
 * File: /ScrumFlix/Models/Location.cs
 * Description: Represents a ScrumFlix theater location with address and active status.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A physical theater location in the ScrumFlix chain.
/// </summary>
[Table("locations")]
public class Location
{
    /// <summary>Gets or sets the unique location identifier.</summary>
    [Key]
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>Gets or sets the display name of the location.</summary>
    [Required]
    [MaxLength(50)]
    [Column("location_name")]
    public string LocationName { get; set; } = string.Empty;

    /// <summary>Gets or sets the street address of the location.</summary>
    [Required]
    [MaxLength(255)]
    [Column("location_address")]
    public string LocationAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this location is currently active.</summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<TheaterRoom> TheaterRooms { get; set; } = new List<TheaterRoom>();
    public ICollection<ScheduledShow> ScheduledShows { get; set; } = new List<ScheduledShow>();
    public ICollection<Inventory> InventoryItems { get; set; } = new List<Inventory>();
    public ICollection<ConcessionsPricing> ConcessionsPricings { get; set; } = new List<ConcessionsPricing>();
    public ICollection<ConcessionsSale> ConcessionsSales { get; set; } = new List<ConcessionsSale>();
    public ICollection<ConcessionsOrder> ConcessionsOrders { get; set; } = new List<ConcessionsOrder>();
}
