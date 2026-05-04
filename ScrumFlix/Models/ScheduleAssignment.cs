using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ScrumFlix.Models;

public class ScheduleAssignment
{
    [Key]
    public int AssignmentId { get; set; }

    public string AssignmentName { get; set; } = "";

    public int UserId { get; set; }
    public User? User { get; set; }

    public int ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public int? ShowtimeId { get; set; }
    public Showtime? Showtime { get; set; }
}
