using System;
using System.Collections.Generic;

namespace WebAIrline.Models;

public partial class SoldSeat
{
    public int SoldSeatId { get; set; }

    public int? FlightId { get; set; }

    public int? SeatId { get; set; }

    public virtual Flight? Flight { get; set; }

    public virtual Seat? Seat { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
