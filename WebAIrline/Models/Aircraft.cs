using System;
using System.Collections.Generic;

namespace WebAIrline.Models;

public partial class Aircraft
{
    public int AircraftId { get; set; }

    public string? AircraftName { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
