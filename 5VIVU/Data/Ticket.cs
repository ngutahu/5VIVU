﻿using System;
using System.Collections.Generic;

namespace _5VIVU.Data;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? FlightId { get; set; }

    public int? UserId { get; set; }

    public string? Cmnd { get; set; }

    public string? FullName { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? SeatNumber { get; set; }

    public DateTime TransactionTime { get; set; }

    public decimal TransactionAmount { get; set; }

    public int? AircraftId { get; set; }

    public int? DepartureAirportId { get; set; }

    public int? ArrivalAirportId { get; set; }

    public DateTime? DepartureTime { get; set; }

    public DateTime? ArrivalTime { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual AppUser? User { get; set; }
}
