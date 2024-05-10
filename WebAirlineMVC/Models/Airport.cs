using System;
using System.Collections.Generic;

namespace WebAirlineMVC.Models;

public partial class Airport
{
    public int AirportId { get; set; }

    public string? AirportName { get; set; }

    public string? Location { get; set; }

    public virtual ICollection<Flight> FlightArrivalAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartureAirports { get; set; } = new List<Flight>();
}
