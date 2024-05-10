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
    public class SoldSeatsController : ControllerBase
    {
        private readonly VietJetContext _context;

        public SoldSeatsController(VietJetContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllSoldSeats()
        {
            return await _context.SoldSeats
                         .Select(s => new
                         {                             
                             soldSeatId= s.SoldSeatId,
                             flightId= s.FlightId,
                             seatId= s.SeatId
                         })
                         .ToListAsync();
        }

        /*[HttpGet("getSoldSeat/{flightId}")]
        public async Task<ActionResult<IEnumerable<SoldSeat>>> getAllSoldSeatsByFlight(int flightId)
        {
            var soldSeats= await _context.SoldSeats
                                 .Where(s=> s.FlightId==flightId)
                                 .Select(s =>
                                 {
                                    soldSeatId: s.SoldSeatId;
                                    flightID: 2,
                                    seatId: 3,
                                 })
                                 .ToListAsync();
            if (soldSeats.Any())
            {
                return(soldSeats);
            }
            else return NotFound();

        }*/

        [HttpGet("getSoldSeat/{flightId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllSoldSeatsByFlight(int flightId)
        {
            var soldSeats = await _context.SoldSeats
                                    .Where(s => s.FlightId == flightId)
                                    .Select(s => new
                                    {
                                        soldSeatId = s.SoldSeatId,
                                        flightId = flightId,
                                        seatId = s.SeatId,
                                        tickets = s.Tickets.Select(t => new
                                        {
                                            ticketId = t.TicketId,
                                            seatNumber = t.SeatNumber,
                                            price = t.Price,
                                            isBooked = t.IsBooked,
                                            fullName = t.FullName,
                                            identificationNumber = t.IdentificationNumber
                                        }).ToList()
                                    })
                                    .ToListAsync();
            if (soldSeats.Any())
            {
                return Ok(soldSeats);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SoldSeat>> GetSoldSeat(int id)
        {
            var soldSeat = await _context.SoldSeats.FindAsync(id);

            if (soldSeat == null)
            {
                return NotFound();
            }

            return soldSeat;
        }
        
        /*[HttpPut("{id}")]
        public async Task<IActionResult> PutSoldSeat(int id, SoldSeat soldSeat)
        {
            if (id != soldSeat.SoldSeatId)
            {
                return BadRequest();
            }

            _context.Entry(soldSeat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SoldSeatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // POST: api/SoldSeats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPost]
        public async Task<ActionResult<SoldSeat>> PostSoldSeat(SoldSeat soldSeat)
        {
            _context.SoldSeats.Add(soldSeat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSoldSeat", new { id = soldSeat.SoldSeatId }, soldSeat);
        }
        */
        // DELETE: api/SoldSeats/5
        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSoldSeat(int id)
        {
            var soldSeat = await _context.SoldSeats.FindAsync(id);
            if (soldSeat == null)
            {
                return NotFound();
            }

            _context.SoldSeats.Remove(soldSeat);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

        private bool SoldSeatExists(int id)
        {
            return _context.SoldSeats.Any(e => e.SoldSeatId == id);
        }
    }
}
