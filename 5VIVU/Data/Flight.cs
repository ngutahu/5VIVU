namespace _5VIVU.Data
{
    public class Flight
    {
        public int FlightId { get; set; }
        public int AircraftId { get; set; }
        public string AircraftName { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public string DepartureAirportName { get; set; }
        public string ArrivalAirportName { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; } 
        public int? TotalSeats { get; set; }
        public int? TotalEmptySeat { get; set; }
        public decimal Price { get; set; }
    }
}
