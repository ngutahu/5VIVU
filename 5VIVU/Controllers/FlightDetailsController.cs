using _5VIVU.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace _5VIVU.Controllers
{
    public class FlightDetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authentication]
        public IActionResult FlightDetail(int id)
        {
            ViewData["flightId"] = id;
            return View();
        }
    }
}
