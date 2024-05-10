using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAIrline.Models;

namespace WebAIrline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly VietJetContext _context;

        public FlightsController(VietJetContext context)
        {
            _context = context;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> getAllFlights()
        {
            var allFlights= await _context.Flights
                    .Select(f => new
                    {
                        flightId = f.FlightId,
                        aircraftId = f.AircraftId,
                        aircraftName = f.Aircraft.AircraftName,
                        departureAirportName = f.DepartureAirport.AirportName,
                        arrivalAirPortName = f.ArrivalAirport.AirportName,
                        departureTime = f.DepartureTime,
                        arrivalTime = f.ArrivalTime,
                        totalSeats = f.TotalSeats
                    })
                    .ToListAsync();
            return allFlights;
        }

        
        [HttpGet("searchFlights")]
        public async Task<ActionResult<IEnumerable<object>>> searchFlights(int departureAirport, int arrivalAirport, DateTime departureTime)
        {
            var flights = await _context.Flights
                            .Where(f => f.DepartureAirportId == departureAirport &&
                                        f.ArrivalAirportId == arrivalAirport &&
                                        f.DepartureTime.Date == departureTime.Date)
                            .Select(f => new
                            {
                                flightId = f.FlightId,
                                aircraftId = f.AircraftId,
                                aircraftName = f.Aircraft.AircraftName,
                                departureAirportName = f.DepartureAirport.AirportName,
                                arrivalAirPortName = f.ArrivalAirport.AirportName,
                                departureTime = f.DepartureTime,
                                arrivalTime = f.ArrivalTime,
                                totalSeats = f.TotalSeats
                            })
                            .ToListAsync();

            if (flights.Any())
            {
                return flights;
            }
            else
            {
                return NotFound("Not Found Any Flights");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> getFlightById(int id)
        {
            var flight = await _context.Flights
                .Where(f=>f.FlightId==id)
                .Select(f => new
                {
                    flightId = f.FlightId,
                    aircraftId = f.AircraftId,
                    aircraftName = f.Aircraft.AircraftName,
                    departureAirportId = f.DepartureAirportId,
                    arrivalAirPortId = f.ArrivalAirportId,
                    departureAirportName = f.DepartureAirport.AirportName,
                    arrivalAirPortName = f.ArrivalAirport.AirportName,
                    departureTime = f.DepartureTime,
                    arrivalTime = f.ArrivalTime,
                    totalSeats = f.TotalSeats, 
                    totalEmptySeat= _context.SoldSeats
                                                .Where(t=> t.FlightId==f.FlightId && !t.Tickets.Any(tk=>tk.IsBooked==true))                                                
                                                .Count(),
                    price= _context.Tickets
                                  .Where(t => t.SoldSeat.FlightId == f.FlightId && t.IsBooked == false)
                                  .Min(t=>t.Price)
                })
                .FirstOrDefaultAsync();

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }
       
        /*[HttpPut("{id}")]
        public async Task<IActionResult> updateFlight(int id, Flight flight)
        {
            if (id != flight.FlightId)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
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
        */


        /*[HttpPost]
        public async Task<ActionResult<Flight>> CreateFlight(Flight flight)
        {
            if (flight == null)
            {
                return NoContent();
            }

            _context.Flights.Add(flight);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return CreatedAtAction(nameof(getFlightById), new { id = flight.FlightId }, flight);
            }
            else
            {
                return NotFound("Cannot create flight");
            }
        }
        */
        
        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.FlightId == id);
        }
    }
}
