using System.Data.SqlClient;

namespace SignUp.Models;

public class BookingModel
{
    public string Email { get; set; }

    public int TicketID { get; set; }
    public int FlightID { get; set; }
    public DateTime BookingTime { get; set; }
    public decimal Amount { get; set; }
    public BookingModel(SqlDataReader reader)
    {
        Email = reader["Email"].ToString() ?? string.Empty;
        TicketID = reader.GetInt32(reader.GetOrdinal("TicketID"));
        FlightID = reader.GetInt32(reader.GetOrdinal("FlightID"));
        BookingTime = reader.GetDateTime(reader.GetOrdinal("TransactionTime"));
        Amount = reader.GetDecimal(reader.GetOrdinal("TransactionAmount"));
    }
}
