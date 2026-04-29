// This model represents a movie and is used by EF Core to map the Movie class to the database

using System.ComponentModel.DataAnnotations;

namespace ScrumFlix.Models;

public class Movie
{
    public int MovieId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(20)]
    public string? Rating { get; set; }
    
    [MaxLength(30)]
    public string? Genre { get; set; }

    public int RuntimeMinutes { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
