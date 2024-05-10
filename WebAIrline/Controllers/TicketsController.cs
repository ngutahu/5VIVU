using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAIrline.Models;

namespace WebAIrline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly VietJetContext _context;

        public TicketsController(VietJetContext context)
        {
            _context = context;
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllTickets(int flightId)
        {
            var listTicket = await _context.Tickets
                                .Where(t => t.SoldSeat.FlightId == flightId)
                                .Select(t => new
                                {
                                    seatNumber = t.SeatNumber,
                                    price = t.Price,
                                    isBooked = t.IsBooked
                                })
                                .ToListAsync();

            if (listTicket.Any())
            {
                return listTicket;
            }
            else
            {
                return NotFound("There are no tickets for this flight.");
            }
        }

        //get all ticket from flight/{id} where seatsold is null
        // GET: api/Tickets
        [HttpGet("unbooked")]
        public async Task<ActionResult<IEnumerable<object>>> GetTickets(int flightId)
        {
            var listTicket = await _context.Tickets
                                .Where(t => t.SoldSeat.FlightId == flightId && t.IsBooked == false)
                                .Select(t => new
                                {
                                    seatNumber = t.SeatNumber,
                                    price = t.Price,
                                    isBooked = t.IsBooked
                                })
                                .ToListAsync();

            if (listTicket.Any())
            {
                return listTicket;
            }
            else
            {
                return NotFound("There are no tickets for this flight.");
            }
        }


        [HttpGet("booked")]
        public async Task<ActionResult<IEnumerable<object>>> GetTicketsIsBooked(int flightId)
        {
            var listTicket = await _context.Tickets
                                .Where(t => t.SoldSeat.FlightId == flightId && t.IsBooked == true)
                                .Select(t => new
                                {
                                    seatNumber = t.SeatNumber,
                                    price = t.Price,
                                    isBooked = t.IsBooked
                                })
                                .ToListAsync();

            if (listTicket.Any())
            {
                return listTicket;
            }
            else
            {
                return NotFound("There are no tickets for this flight.");
            }
        }
        /*private async Task<List<object>> getAllTicketByFlightId(int flightId)
        {
            var listTicket = await _context.Tickets
                                .Where(t => t.SoldSeat.FlightId == flightId)
                                .Select(t => new
                                {
                                    seatNumber = t.SeatNumber,
                                    price = t.Price,
                                    condition = t.IsBooked
                                })
                                .ToListAsync();
            if (listTicket.Any())
            {
                return listTicket;
            }
            else
            {
                return NotFound("There are no tickets for this flight.");
            }
        }*/


        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }




        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return BadRequest();
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tickets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicket", new { id = ticket.TicketId }, ticket);
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}