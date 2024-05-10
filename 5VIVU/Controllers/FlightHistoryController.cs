 using _5VIVU.Data;
using _5VIVU.Models;
using _5VIVU.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace _5VIVU.Controllers
{
    public class FlightHistoryController : Controller
    {
        private readonly _5vivuBanveContext _context;

        public FlightHistoryController(_5vivuBanveContext context)
        {
            _context = context;
        }
        [Authentication]
        public IActionResult ViewHistory(int? userId)
        {
            var tickets = _context.Tickets
                                  .Where(t => t.UserId == userId);
            var result = tickets.Select(t => new TicketVM
            {
                TicketId=t.TicketId,
                UserId = t.UserId,
                Phone = t.Phone,
                SeatNumber = t.SeatNumber,
                FullName=t.FullName,
            }
            );
            return View(result);
        }
        [Authentication]

        public IActionResult Details(int? TicketId)
        {
            var tickets = _context.Tickets
                               .Where(t => t.TicketId == TicketId);
            var result2 = tickets.Select(t => new TicketVM
            {
                TicketId = t.TicketId,
                FlightId = t.FlightId,
                UserId = t.UserId,
                FullName=t.FullName,
                Phone=t.Phone,
                Email=t.Email,
                Cmnd=t.Cmnd,
                Birthday=t.Birthday,
                SeatNumber = t.SeatNumber,
                TransactionTime = t.TransactionTime,
                TransactionAmount = t.TransactionAmount,
                AircraftId = t.AircraftId,
                DepartureAirportId = t.DepartureAirportId,
                ArrivalAirportId = t.ArrivalAirportId,
                DepartureTime = t.DepartureTime,
                ArrivalTime= t.ArrivalTime,
                Price=t.Price,

            }
            );
            return View(result2);
        }

       
        [Authentication]
        public IActionResult CancelTicket(int ticketId)
        {
            var ticketsCancel = _context.Tickets.FirstOrDefault(t => t.TicketId == ticketId);
            if (ticketsCancel != null)
            {
                _context.Tickets.Remove(ticketsCancel);
                _context.SaveChanges();
                return RedirectToAction("ViewHistory", new { userId = ticketsCancel.UserId });
            }
            else
            {
                return NotFound();
            }
        }
    }
}
