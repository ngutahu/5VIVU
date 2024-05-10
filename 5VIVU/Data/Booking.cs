using System;
using System.Collections.Generic;

namespace _5VIVU.Data;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? TicketId { get; set; }

    public DateTime? BookingTime { get; set; }

    public bool? IsSuccess { get; set; }

    public decimal? Amount { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
