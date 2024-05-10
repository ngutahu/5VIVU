using _5VIVU.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace _5VIVU.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authentication]
        public IActionResult AdminHome()
        {
            return View();
        }
    }
}
