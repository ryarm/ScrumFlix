/*
 * File: /ScrumFlix/Models/Movie.cs
 * Description: Represents a movie available in the ScrumFlix system with metadata for browsing.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A movie that can be scheduled and shown at ScrumFlix theaters.
/// </summary>
[Table("movies")]
public class Movie
{
    /// <summary>Gets or sets the unique movie identifier.</summary>
    [Key]
    [Column("movie_id")]
    public int MovieId { get; set; }

    /// <summary>Gets or sets the title of the movie.</summary>
    [Required]
    [MaxLength(150)]
    [Column("movie_name")]
    [Display(Name = "Title")]
    public string MovieName { get; set; } = string.Empty;

    /// <summary>Gets or sets the genre category (e.g., Action, Comedy, Sci-Fi).</summary>
    [Required]
    [MaxLength(50)]
    [Column("genre")]
    public string Genre { get; set; } = string.Empty;

    /// <summary>Gets or sets the MPA/MPAA rating (e.g., PG, PG-13, R).</summary>
    [Required]
    [MaxLength(10)]
    [Column("mpa_rating")]
    [Display(Name = "Rating")]
    public string MpaRating { get; set; } = string.Empty;

    /// <summary>Gets or sets the runtime in minutes.</summary>
    [Column("run_time")]
    [Display(Name = "Runtime (min)")]
    [Range(1, 9999)]
    public short RunTime { get; set; }

    // Navigation properties
    public ICollection<ScheduledShow> ScheduledShows { get; set; } = new List<ScheduledShow>();

    /// <summary>Formats runtime as hours and minutes string.</summary>
    /// <returns>Human-readable runtime string like "2h 6m".</returns>
    [NotMapped]
    public string FormattedRunTime =>
        RunTime >= 60
            ? $"{RunTime / 60}h {RunTime % 60}m"
            : $"{RunTime}m";
}
