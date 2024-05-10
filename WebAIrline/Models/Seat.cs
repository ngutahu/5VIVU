using System;
using System.Collections.Generic;

namespace WebAIrline.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int? AircraftId { get; set; }

    public String? SeatNumber { get; set; }

    public virtual Aircraft? Aircraft { get; set; }

    public virtual ICollection<SoldSeat> SoldSeats { get; set; } = new List<SoldSeat>();
}
