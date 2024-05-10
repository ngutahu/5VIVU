using _5VIVU.Data;
using _5VIVU.Models;
using _5VIVU.Models.Authentication;
using _5VIVU.Services;
using _5VIVU.VIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace _5VIVU.Controllers
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
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TotalSeats { get; set; }
        public int TotalEmptySeat { get; set; }
        public decimal Price { get; set; }
    }
    public class BookingController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly _5vivuBanveContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _sendmail;
        private readonly HttpClient _client;

        public BookingController(_5vivuBanveContext context, IVnPayService vnPayService, IWebHostEnvironment env, IEmailSender sendMailService, HttpClient client)
        {
            _vnPayService = vnPayService;
            _context = context;
            _env = env;
            _sendmail = sendMailService;
            _client = client;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authentication]
        [HttpGet]
        public IActionResult Book(int flightId)
        {
            ViewData["flightId"] = flightId;
            return View();
        }

        [Authentication]
        [HttpPost]
        public IActionResult Book(decimal price, string fullName, string iNumber, string email, string selectedSeat, string flightId)
        {
            if (!ModelState.IsValid)
            {
                return Index();
            }

            TempData["Email"] = email;
            TempData["FlightId"] = Convert.ToInt32(flightId);
            TempData["Name"] = fullName;
            TempData["iNumber"] = iNumber;
            TempData["selectedSeat"] = selectedSeat;
            TempData["Price"] = price.ToString();

            var vnPayModel = new VnPayMentRequestModel
            {
                amount = price,
                createDate = DateTime.Now,
                description = $"{fullName} - {iNumber}",
                fullName = fullName,
                ticketID = new Random().Next(1000, 100000)
            };

            return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
        }

        [Authentication]
        public async Task<IActionResult> PaymentFail()
        {
            return View("ErrorBooking");
        }


        [Authentication]
        public async Task<IActionResult> PayMentCallBack()
        {
            try
            {
                var response = _vnPayService.PaymentExcute(Request.Query);

                if (response == null || response.VnPayResponseCode != "00")
                {
                    return View("ErrorBooking");
                }
                await _sendmail.SendEmailAsync($"{TempData["Email"]}",
                                                "Thanh toán thành công",
                                                $"<p>Kính gửi {TempData["Name"]},</p></br>" +
                                                $"<p>Cảm ơn quý khách đã mua vé máy bay tại đại lí của chúng tôi, chúng tôi vô cùng biết ơn sự tin tưởng của quí khách!!!</p>" +
                                                $"<p>Thông tin đặt vé:</p>" +
$"<p>- Họ và tên: {TempData["Name"]}</p>" +
$"<p>- Số CMND: {TempData["iNumber"]}</p>" +
$"<p>- Chuyến bay: {TempData["FlightId"]}</p>" +
$"<p>- Số ghế: {TempData["selectedSeat"]}</p>" +
$"<p>- Giá vé: {TempData["Price"]}</p>");

                ViewData["email"] = TempData["Email"];
                ViewData["flightId"] = TempData["FlightId"];
                ViewData["name"] = TempData["Name"];
                ViewData["identificationNumber"] = TempData["iNumber"];
                ViewData["price"] = TempData["Price"];
                ViewData["selectedSeat"] = TempData["selectedSeat"];
                var flightId = (int)TempData["FlightId"];
                if (await AddNewTicket(flightId))
                    return View("Success");
                else
                    return View("ErrorBooking");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return View("ErrorBooking");
            }
        }
        private async Task<bool> AddNewTicket(int flightId)
        {
            HttpResponseMessage apiResponse = await _client.GetAsync($"https://localhost:7019/api/Flights/{flightId}");
            apiResponse.EnsureSuccessStatusCode();
            string responseBody = await apiResponse.Content.ReadAsStringAsync();

            int userId = HttpContext.Session.GetInt32("Id").Value;

            var user = await _context.AppUsers
                               .Where(u => u.Id == userId)
                               .FirstOrDefaultAsync();

            var flight = JsonConvert.DeserializeObject<Flight>(responseBody);

            var transAmount = Convert.ToInt32(TempData["Price"]);

            var ticket = new Ticket
            {
                FlightId = flight.FlightId,
                UserId = userId,
                Cmnd = TempData["iNumber"].ToString(),
                FullName = user.FullName,
                Birthday = user.Birthday,
                Phone = user.Phone,
                Email = user.Email,
                SeatNumber = TempData["selectedSeat"].ToString(),
                TransactionTime = DateTime.Now,
                TransactionAmount = transAmount,
                AircraftId = flight.AircraftId,
                DepartureAirportId = flight.DepartureAirportId,
                ArrivalAirportId = flight.ArrivalAirportId,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                Price = transAmount
            };

            _context.Tickets.Add(ticket);
            var res = await _context.SaveChangesAsync();

            return res > 0;
        }
    }
}
