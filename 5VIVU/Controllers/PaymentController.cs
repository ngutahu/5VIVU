using Microsoft.AspNetCore.Mvc;

namespace _5VIVU.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult BookingSuccess(int flightID)
        {
            ViewData["flightId"] = flightID;
            ViewBag.FullName = TempData["FullName"];
            ViewBag.Birthday = TempData["Birthday"];
            ViewBag.Cmnd = TempData["Cmnd"];
            ViewBag.Email = TempData["Email"];
            ViewBag.Phone = TempData["Phone"];

            return View();
        }
    }
}
