// This model represents a showtime and is used by EF Core to map the Showtime class to the database

using System.ComponentModel.DataAnnotations;
using System;

namespace ScrumFlix.Models;

public class Showtime
{
    public int ShowtimeId { get; set; }
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }
    public int TheaterScreenId { get; set; }
    public TheaterScreen? TheaterScreen { get; set; }
    public DateTime StartTime { get; set; }

}
