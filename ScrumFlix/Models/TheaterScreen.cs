using System.ComponentModel.DataAnnotations;

namespace ScrumFlix.Models;

public class TheaterScreen
{
    public int TheaterScreenId { get; set; }

    [Required, MaxLength(100)]
    public string ScreenName { get; set; } = "";
}