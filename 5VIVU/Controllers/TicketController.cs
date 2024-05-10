using Microsoft.AspNetCore.Mvc;

namespace _5VIVU.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListTicket()
        {
            return View();
        }
    }
}
