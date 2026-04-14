using System.ComponentModel.DataAnnotations;
using System;

namespace ScrumFlix.Models;

public class Ticket
{
    public int TicketId { get; set; }
    public int TicketCode { get; set; }

    public int ShowtimeId { get; set; }
    public Showtime? Showtime { get; set; }
    public int UserAtSale { get; set; }
    public User? User { get; set; }
    public DateTime TimeOfSale { get; set; }
}
