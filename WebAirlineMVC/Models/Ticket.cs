using System;
using System.Collections.Generic;

namespace WebAirlineMVC.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? SoldSeatId { get; set; }

    public string? SeatNumber { get; set; }

    public decimal? Price { get; set; }

    public bool? IsBooked { get; set; }

    public string? FullName { get; set; }

    public string? IdentificationNumber { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual SoldSeat? SoldSeat { get; set; }
}
