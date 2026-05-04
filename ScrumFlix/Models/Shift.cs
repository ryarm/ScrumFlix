using System;
using System.Collections.Generic;
using System.Text;

namespace ScrumFlix.Models;

public class Shift
{
    public int ShiftId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }
}
