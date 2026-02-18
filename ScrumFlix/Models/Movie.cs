using System.ComponentModel.DataAnnotations;

namespace ScrumFlix.Models;

public class Movie
{
    public int MovieId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(20)]
    public string? Rating { get; set; }

    public int RuntimeMinutes { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
