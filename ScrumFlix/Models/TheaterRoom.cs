/*
 * File: /ScrumFlix/Models/TheaterRoom.cs
 * Description: Represents a screening room within a theater location, including seating capacity.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A physical screening room inside a ScrumFlix theater location.
/// </summary>
[Table("theater_rooms")]
public class TheaterRoom
{
    /// <summary>Gets or sets the unique room identifier.</summary>
    [Key]
    [Column("room_id")]
    public int RoomId { get; set; }

    /// <summary>Gets or sets the location this room belongs to.</summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>Gets or sets the display name of the room (e.g., "IMAX 1", "Screen 2").</summary>
    [Required]
    [MaxLength(50)]
    [Column("room_name")]
    [Display(Name = "Room Name")]
    public string RoomName { get; set; } = string.Empty;

    /// <summary>Gets or sets the total number of seats in this room.</summary>
    [Column("seating_capacity")]
    [Display(Name = "Seating Capacity")]
    [Range(1, int.MaxValue)]
    public int SeatingCapacity { get; set; }

    /// <summary>Gets or sets whether this room is currently active.</summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }
    public ICollection<ScheduledShow> ScheduledShows { get; set; } = new List<ScheduledShow>();
}
