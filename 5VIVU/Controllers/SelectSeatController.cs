using Microsoft.AspNetCore.Mvc;
using _5VIVU.Data;

namespace _5VIVU.Controllers
{
    public class SelectSeatController : Controller
    {
        [HttpGet]
        public IActionResult PickSeat(int flightId)
        {
            ViewData["flightId"] = flightId;
            return View();
        }

        [HttpPost]
        public IActionResult BookTicket(Ticket ticketInfo)
        {
            return RedirectToAction("Payment", "Booking", new { ticketId = ticketInfo.TicketId });
        }
    }
}
