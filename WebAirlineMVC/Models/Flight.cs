using System;
using System.Collections.Generic;

namespace WebAirlineMVC.Models;

public partial class Flight
{
    public int FlightId { get; set; }

    public int? AircraftId { get; set; }

    public int? DepartureAirportId { get; set; }

    public int? ArrivalAirportId { get; set; }

    public DateTime DepartureTime { get; set; }

    public DateTime? ArrivalTime { get; set; }

    public int? TotalSeats { get; set; }

    public virtual Aircraft? Aircraft { get; set; }

    public virtual Airport? ArrivalAirport { get; set; }

    public virtual Airport? DepartureAirport { get; set; }

    public virtual ICollection<SoldSeat> SoldSeats { get; set; } = new List<SoldSeat>();
}
