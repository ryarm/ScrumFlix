/*
 * File: /ScrumFlix/Models/ScheduledShow.cs
 * Description: Represents a specific scheduled screening of a movie at a location and room with date/time.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A scheduled showing of a movie at a specific room and time.
/// </summary>
[Table("scheduled_shows")]
public class ScheduledShow
{
    /// <summary>Gets or sets the unique show identifier.</summary>
    [Key]
    [Column("show_id")]
    public int ShowId { get; set; }

    /// <summary>Gets or sets the movie being shown.</summary>
    [Column("movie_id")]
    public int MovieId { get; set; }

    /// <summary>Gets or sets the theater location.</summary>
    [Column("location_id")]
    public int LocationId { get; set; }

    /// <summary>Gets or sets the screening room.</summary>
    [Column("room_id")]
    public int RoomId { get; set; }

    /// <summary>Gets or sets the date of the show.</summary>
    [Column("show_date")]
    [Display(Name = "Show Date")]
    public DateTime ShowDate { get; set; }

    /// <summary>Gets or sets the show start time.</summary>
    [Column("start_datetime")]
    [Display(Name = "Start Time")]
    public DateTime StartDateTime { get; set; }

    /// <summary>Gets or sets the show end time.</summary>
    [Column("end_datetime")]
    [Display(Name = "End Time")]
    public DateTime EndDateTime { get; set; }

    /// <summary>Gets or sets whether this show is active and available for booking.</summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets number of tickets sold for this show.</summary>
    [Column("tickets_sold")]
    [Display(Name = "Tickets Sold")]
    public int TicketsSold { get; set; }

    // Navigation properties
    [ForeignKey(nameof(MovieId))]
    public Movie? Movie { get; set; }

    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }

    [ForeignKey(nameof(RoomId))]
    public TheaterRoom? TheaterRoom { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>Returns remaining seats for this show.</summary>
    [NotMapped]
    public int AvailableSeats => (TheaterRoom?.SeatingCapacity ?? 0) - TicketsSold;
}
