using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAirlineMVC.Models;

namespace WebAirlineMVC.Controllers
{   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public JsonResult getName()
        {
            var names = new string[3]
            {
                "nguyen","tan","huy"
            };
            return new JsonResult(Ok(names));

        }
        public JsonResult postName(string name)
        {
            return new JsonResult(Ok());
        }

    }
}
