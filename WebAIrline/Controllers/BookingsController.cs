using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAIrline.Models;
using WebAIrline.Services;

namespace WebAIrline.Controllers
{
    public class BookingData
    {
        public int FlightId { get; set; }
        public decimal Amount { get; set; }
        public string FullName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Email { get; set; }
        public string SeatNumber {  get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly VietJetContext _context;
        private readonly IEmailSender _emailSender;

        public BookingsController(VietJetContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _context.Bookings
                .Where(b => b.IsSuccess == true)
                .ToListAsync();
            if (!bookings.Any())
            {
                return NotFound("Không có booking nào hoàn tất.");
            }
            return bookings;
        }

        // GET: api/Bookings/isNotSusscess
        [HttpGet("isNotSusscess")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingUnsuccess()
        {
            var bookings = await _context.Bookings
                .Where(b => b.IsSuccess == false)
                .ToListAsync();
            if (!bookings.Any())
            {
                return NotFound("Không có booking nào chưa hoàn tất.");
            }
            return bookings;
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound("Không tìm thấy booking này.");
            }

            return booking;
        }

        // POST: api/Bookings/createBooking
        [HttpPost("createBooking")]
        public async Task<ActionResult> CreateBooking([FromBody] BookingData bookingData)
        {
            //Console.WriteLine("post api.....");
            if (bookingData == null ||
                bookingData.FlightId <= 0 ||
                string.IsNullOrEmpty(bookingData.FullName) ||
                string.IsNullOrEmpty(bookingData.IdentificationNumber) ||
                string.IsNullOrEmpty(bookingData.Email))
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var flight = await _context.Flights
                .Where(f => f.FlightId == bookingData.FlightId)
                .Select(f => new
                {
                    aircraftName = f.Aircraft.AircraftName,
                    departureLocation = f.DepartureAirport.Location,
                    departureAirportName = f.DepartureAirport.AirportName,
                    arrivalLocation = f.ArrivalAirport.Location,
                    arrivalAirPortName = f.ArrivalAirport.AirportName,
                    departureTime = f.DepartureTime,
                    arrivalTime = f.ArrivalTime
                })
                .FirstOrDefaultAsync();

            if (flight == null)
            {
                return NotFound("Không tìm thấy thông tin về chuyến bay.");
            }
            var ticket = await _context.Tickets
                .Where(t => t.SoldSeat.FlightId == bookingData.FlightId && t.SeatNumber==bookingData.SeatNumber)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound("Không có vé khả dụng cho chuyến bay này.");
            }
            if (ticket.IsBooked == false)
            {
                var booking = new Booking
                {
                    TicketId = ticket.TicketId,
                    BookingTime = DateTime.Now,
                    IsSuccess = false,
                    Amount = bookingData.Amount
                };
                await _context.AddAsync(booking);
                await _context.SaveChangesAsync();
                if (booking.Amount >= ticket.Price)
                {
                    ticket.IsBooked = true;
                    ticket.FullName = bookingData.FullName;
                    ticket.IdentificationNumber = bookingData.IdentificationNumber;
                    _context.Update(ticket);
                    booking.IsSuccess = true;
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest("Số tiền không đủ để thực hiện giao dịch.");
                }

                await _emailSender.SendEmailAsync(bookingData.Email, "Xác nhận đặt vé thành công",
                    $"<p>Kính gửi {bookingData.FullName},</p></br>" +
                    $"<p>Chúng tôi xin gửi lời cảm ơn chân thành nhất đến {bookingData.FullName} vì đã tin tưởng và chọn lựa dịch vụ của chúng tôi cho chuyến đi vừa qua. Sự ủng hộ và lòng tin của quý khách là nguồn động viên lớn lao giúp chúng tôi nỗ lực hơn nữa để mang đến những trải nghiệm tốt nhất.</p></br>" +
                    $"<p>Thông tin chuyến bay:</p></br>" +
                    $"<p>Từ: {flight.departureLocation}({flight.departureAirportName})</p></br>" +
                    $"<p>Đến: {flight.arrivalLocation}({flight.arrivalAirPortName})</p></br>" +
                    $"<p>Thời gian cất cánh: {flight.departureTime}</p></br>" +
                    $"<p>Thời gian hạ cánh {flight.arrivalTime}</p></br>" +
                    $"<p>Vị trí ghế ngồi {ticket.SeatNumber}</p></br>" +
                    $"<p>Qúy khách vui lòng đến trước giờ bay 1 tiếng rữi để hoàn thành thủ tục bay</p></br>" +
                    $"<p>Trân trọng,</p></br>" +
                    $"VietJet");

                return Ok(ticket.SeatNumber);
            }
            else
            {
                var newTicket = await _context.Tickets
                                                .Where(t => t.SoldSeat.FlightId == bookingData.FlightId && t.IsBooked == false)
                                                .FirstOrDefaultAsync();
                var booking = new Booking
                {
                    TicketId = newTicket.TicketId,
                    BookingTime = DateTime.Now,
                    IsSuccess = false,
                    Amount = bookingData.Amount
                };
                await _context.AddAsync(booking);
                await _context.SaveChangesAsync();
                if (booking.Amount >= newTicket.Price)
                {
                    newTicket.IsBooked = true;
                    newTicket.FullName = bookingData.FullName;
                    newTicket.IdentificationNumber = bookingData.IdentificationNumber;
                    _context.Update(newTicket);
                    booking.IsSuccess = true;
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest("Số tiền không đủ để thực hiện giao dịch.");
                }

                await _emailSender.SendEmailAsync(bookingData.Email, "Xác nhận đặt vé thành công",
                    $"<p>Kính gửi {bookingData.FullName},</p></br>" +
                    $"<p>Chúng tôi xin gửi lời cảm ơn chân thành nhất đến {bookingData.FullName} vì đã tin tưởng và chọn lựa dịch vụ của chúng tôi cho chuyến đi vừa qua. Sự ủng hộ và lòng tin của quý khách là nguồn động viên lớn lao giúp chúng tôi nỗ lực hơn nữa để mang đến những trải nghiệm tốt nhất.</p></br>" +
                    $"<p>Thông tin chuyến bay:</p></br>" +
                    $"<p>Từ: {flight.departureLocation}({flight.departureAirportName})</p></br>" +
                    $"<p>Đến: {flight.arrivalLocation}({flight.arrivalAirPortName})</p></br>" +
                    $"<p>Thời gian cất cánh: {flight.departureTime}</p></br>" +
                    $"<p>Thời gian hạ cánh {flight.arrivalTime}</p></br>" +
                    $"<p>Vị trí ghế ngồi {newTicket.SeatNumber}</p></br>" +
                    $"<p>Qúy khách vui lòng đến trước giờ bay 1 tiếng rữi để hoàn thành thủ tục bay</p></br>" +
                    $"<p>Trân trọng,</p></br>" +
                    $"VietJet");

                return Ok(newTicket.SeatNumber);
            }
            
        }

        // POST: api/Bookings/cancelBooking
        [HttpPost("cancelBooking")]
        public async Task<ActionResult> CancelBooking(int ticketId)
        {
            var ticket = await _context.Tickets
                .Where(b => b.TicketId == ticketId)
                .FirstOrDefaultAsync();
            if (ticket == null)
            {
                return BadRequest("Booking không tồn tại.");
            }
            if (ticket.IsBooked == false)
            {
                return BadRequest("Booking cho vé chưa thành công nên không thể hủy.");
            }
            ticket.IsBooked = false;
            ticket.FullName = null;
            ticket.IdentificationNumber = null;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
            var booking = await _context.Bookings
                .Where(b => b.TicketId == ticket.TicketId && b.IsSuccess == true)
                .FirstOrDefaultAsync();
            if (booking != null)
            {
                _context.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return Ok("Hủy thành công!!!");
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
