using _5VIVU.Data;
using _5VIVU.Models;
using _5VIVU.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _5VIVU.Controllers
{
    public class AmountTicketController : Controller
    {
        private readonly _5vivuBanveContext _context;

        public AmountTicketController(_5vivuBanveContext context)
        {
            _context = context;
        }
        [Authentication]
        public IActionResult AmountTicket()
        {
            int currentYear = DateTime.Now.Year; // hoặc bạn có thể chọn một năm khác
            ViewBag.TotalTicketsSold = GetTotalTicketsSold();
            ViewBag.TotalTransactionAmount = GetTotalTransactionAmount(currentYear);
            return View();

        }
        public IQueryable<TicketVM> GetTickets(int year)
        {
            var tickets = _context.Tickets
        .Where(t => t.TransactionTime.Year == year) // Lọc theo năm
        .Include(t => t.User)
        .Select(t => new TicketVM
        {
            TicketId = t.TicketId,
            FlightId = t.FlightId,
            UserId = t.UserId,
            Cmnd = t.Cmnd,
            FullName = t.FullName,
            Birthday = t.Birthday,
            Phone = t.Phone,
            Email = t.Email,
            SeatNumber = t.SeatNumber,
            TransactionTime = t.TransactionTime,
            TransactionAmount = t.TransactionAmount,
            AircraftId = t.AircraftId,
            DepartureAirportId = t.DepartureAirportId,
            ArrivalAirportId = t.ArrivalAirportId,
            DepartureTime = t.DepartureTime,
            ArrivalTime = t.ArrivalTime,
            Price = t.Price,
            Bookings = t.Bookings,
            AppUser = t.User
        });

            return tickets;
        }


        public int GetTotalTicketsSold()
        {
            int totalTicketsSold = _context.Tickets.Count();

            return totalTicketsSold;
        }

        public decimal GetTotalTransactionAmount(int year)
        {
            return GetTickets(year)
                    .Where(t => t.TransactionTime != null && t.TransactionTime.Year == year)
                    .Sum(t => t.TransactionAmount);
        }

        [HttpPost]
        public IActionResult AmountTicketByMonth(string? year)
        {
            int yearValue = 0;
            List<string> month1 = new List<string>();

            List<decimal> monthlyTransactionAmounts = new List<decimal>();
            List<int> monthlyTicketCounts = new List<int>();
            if (!string.IsNullOrEmpty(year) && int.TryParse(year, out int parsedYear))
            {
                yearValue = parsedYear;
            }
            for (int month = 1; month <= 12; month++)
            {
                DateTime startDate = new DateTime(yearValue, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                decimal totalTransactionAmount = _context.Tickets
                    .Where(t => t.TransactionTime >= startDate && t.TransactionTime <= endDate)
                    .Sum(t => t.TransactionAmount);

                int totalTicketCount = _context.Tickets
                    .Where(t => t.TransactionTime >= startDate && t.TransactionTime <= endDate)
                    .Count();


                monthlyTransactionAmounts.Add(totalTransactionAmount);
                monthlyTicketCounts.Add(totalTicketCount);
                month1.Add(startDate.ToString("MMMM"));
            }

            var result = new { MonthlyTransactionAmounts = monthlyTransactionAmounts, MonthlyTicketCounts = monthlyTicketCounts, Month = month1 };
            return Json(result);
        }


    }
}