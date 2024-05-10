using _5VIVU.Data;

namespace _5VIVU.Models
{
    public class Tickets
    {
        public int TicketId { get; set; }

        public int? FlightId { get; set; }

        public int? UserId { get; set; }

        public string? Cmnd { get; set; }

        public string? FullName { get; set; }

        public DateOnly? Birthday { get; set; }

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

        public DateTime? PaymentTime { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public virtual AppUser? AppUser { get; set; }
    }
}
