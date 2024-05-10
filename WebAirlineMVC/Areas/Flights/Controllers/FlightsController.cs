using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAirlineMVC.Models;

namespace WebAirlineMVC.Areas.Flights.Controllers
{
    [Area("Flights")]
    public class FlightsController : Controller
    {
        private readonly VietJetContext _context;

        public FlightsController(VietJetContext context)
        {
            _context = context;
        }
        [Route("/list-all-flights")]        
        public async Task<IActionResult> Index()
        {
            var vietJetContext = _context.Flights
                                .Include(f => f.Aircraft)
                                .Include(f => f.ArrivalAirport)
                                .Include(f => f.DepartureAirport)
                                .Where(f=>f.DepartureTime.Date > DateTime.Now);
            return View(await vietJetContext.ToListAsync());
        }

        // GET: Flights/Flights/Details/5
        [Route("/flight-details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .FirstOrDefaultAsync(m => m.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Flights/Create
        [Route("/create-flight")]
        public IActionResult Create()
        {
            var aircraftList = _context.Aircraft.Select(a => new SelectListItem
            {
                Value = a.AircraftId.ToString(),
                Text = a.AircraftName
            }).ToList();
            ViewData["AircraftList"] = new SelectList(aircraftList, "Value", "Text");

            var airportList = _context.Airports
                                     .Select(a => new SelectListItem
                                     {
                                         Value = a.AirportId.ToString(),
                                         Text = a.AirportName
                                     })
                                     .ToList();
            ViewData["AirportList"] = new SelectList(airportList, "Value", "Text");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/create-flight")]
        public async Task<IActionResult> Create([Bind("FlightId,AircraftId,DepartureAirportId,ArrivalAirportId,DepartureTime,ArrivalTime,TotalSeats")] Flight flight, [Bind("Price")] decimal price)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var aircraftList = await _context.Aircraft.Select(a => new SelectListItem
                    {
                        Value = a.AircraftId.ToString(),
                        Text = a.AircraftName
                    }).ToListAsync();       
                    var airportList = await _context.Airports
                                                .Select(a => new SelectListItem
                                                {
                                                    Value = a.AirportId.ToString(),
                                                    Text = a.AirportName
                                                })
                                                .ToListAsync();
                    ViewData["AircraftList"] = new SelectList(aircraftList, "Value", "Text");
                    ViewData["AirportList"] = new SelectList(airportList, "Value", "Text");

                    var aircraft = await _context.Aircraft.FirstOrDefaultAsync(a => a.AircraftId == flight.AircraftId);
                    if (aircraft == null)
                    {
                        ModelState.AddModelError("", "Invalid aircraft.");
                        return View(flight);
                    }

                    if (flight.DepartureAirportId == flight.ArrivalAirportId)
                    {
                        ModelState.AddModelError("ArrivalAirportId", "Arrival airport is the same as departure airport.");
                        return View(flight);
                    }

                    var seats = await _context.Seats.Where(s => s.AircraftId == aircraft.AircraftId).ToListAsync();

                    if (seats.Count < flight.TotalSeats)
                    {
                        ModelState.AddModelError("TotalSeats", "The number of total seats exceeds the available seats on the aircraft.");
                        return View(flight);
                    }

                    if (flight.DepartureTime <= DateTime.Now.AddMonths(1))
                    {
                        ModelState.AddModelError("DepartureTime", "Departure time must be at least one month from now.");
                        return View(flight);
                    }

                    if (flight.ArrivalTime <= flight.DepartureTime)
                    {
                        ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
                        return View(flight);
                    }
                    if (_context.Flights.Any(f => f.AircraftId == flight.AircraftId && f.DepartureTime.Date == flight.DepartureTime.Date))
                    {
                        ModelState.AddModelError("DepartureTime", "The aircraft already has a flight scheduled for the departure date.");
                        return View(flight);
                    }

                    _context.Add(flight);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < flight.TotalSeats; i++)
                    {
                        var soldSeat = new SoldSeat
                        {
                            FlightId = flight.FlightId,
                            SeatId = seats[i].SeatId
                        };
                        _context.Add(soldSeat);
                        await _context.SaveChangesAsync();

                        var ticket = new Ticket
                        {
                            SoldSeatId = soldSeat.SoldSeatId,
                            SeatNumber = seats[i].SeatNumber,
                            Price = price,
                            IsBooked = false,
                            FullName = null,
                            IdentificationNumber = null
                        };
                        _context.Add(ticket);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }              
                ViewData["AircraftId"] = new SelectList(_context.Aircraft, "AircraftId", "AircraftName", flight.AircraftId);
                ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportName", flight.ArrivalAirportId);
                ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportName", flight.DepartureAirportId);

                return View(flight);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(flight);
            }
        }

        // POST: Flights/Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/create-flight")]
        public async Task<IActionResult> Create([Bind("FlightId,AircraftId,DepartureAirportId,ArrivalAirportId,DepartureTime,ArrivalTime,TotalSeats")] Flight flight, [Bind("Price")] decimal price)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var aircraft = await _context.Aircraft.FirstOrDefaultAsync(a => a.AircraftId == flight.AircraftId);
                    if (aircraft == null)
                    {
                        ModelState.AddModelError("", "Invalid aircraft.");
                        return View(flight);
                    }

                    if (flight.DepartureAirportId == flight.ArrivalAirportId)
                    {
                        ModelState.AddModelError("Arrival Airport", "Arrival airport is existed.");
                        return View(flight);
                    }

                    if (flight.TotalSeats > aircraft.Capacity || flight.TotalSeats <= 0)
                    {
                        ModelState.AddModelError("TotalSeats", "Total seats cannot exceed aircraft capacity or be less than 1.");
                        return View(flight);
                    }

                    if (flight.DepartureTime.AddMonths(1) < DateTime.Now)
                    {
                        ModelState.AddModelError("DepartureTime", "Departure time must be at least one month from now.");
                        return View(flight);
                    }

                    if (flight.ArrivalTime <= flight.DepartureTime)
                    {
                        ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
                        return View(flight);
                    }

                    _context.Add(flight);
                    _context.SaveChanges();

                    var _aircraft = await _context.Aircraft.FirstOrDefaultAsync(a => a.AircraftId == flight.AircraftId);
                    for (int i = 0; i < flight.TotalSeats; i++)
                    {
                        var seatId = _aircraft.Seats.OrderBy(s => s.SeatId).Skip(i).FirstOrDefault()?.SeatId;
                        var soldSeat = new SoldSeat
                        {
                            FlightId = flight.FlightId,
                            SeatId = seatId
                        };
                        _context.Add(soldSeat);
                        _context.SaveChanges();
                        var seatNumber = _aircraft.Seats.OrderBy(s => s.SeatId).Skip(i).FirstOrDefault()?.SeatNumber;
                        var ticket = new Ticket
                        {
                            SoldSeatId = soldSeat.SoldSeatId,
                            SeatNumber = seatNumber,
                            Price = price, 
                            IsBooked = false,
                            FullName = null,
                            IdentificationNumber = null
                        };
                        _context.Add(ticket);
                        _context.SaveChanges();
                    }
                    return RedirectToAction(nameof(Index));
                }
                ViewData["AircraftId"] = new SelectList(_context.Aircraft, "AircraftId", "AircraftId", flight.AircraftId);
                ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.ArrivalAirportId);
                ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportId", flight.DepartureAirportId);

                return View(flight);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(flight);
            }
        }*/


        // GET: Flights/Flights/Edit/5
        [HttpGet]
        [Route("/edit-flight")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
      
            var aircraftList = await _context.Aircraft
                                            .Select(a => new SelectListItem
                                                {
                                                    Value = a.AircraftId.ToString(),
                                                    Text = a.AircraftName
                                                }).ToListAsync();
            var airportList = await _context.Airports
                                             .Select(a => new SelectListItem
                                                   {
                                                       Value = a.AirportId.ToString(),
                                                       Text = a.AirportName
                                              }).ToListAsync();

            ViewBag.AircraftList = aircraftList;
            ViewBag.AirporList = airportList;

            
            ViewData["AircraftId"] = new SelectList(_context.Aircraft, "AircraftId", "AircraftName", flight.AircraftId);
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportName", flight.ArrivalAirportId);
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportName", flight.DepartureAirportId);

            return View(flight);
        }

        // POST: Flights/Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/edit-flight")]
        public async Task<IActionResult> Edit(/*int id,*/ [Bind("FlightId,AircraftId,DepartureAirportId,ArrivalAirportId,DepartureTime,ArrivalTime,TotalSeats")] Flight flight)
        {
            /*if (id != flight.FlightId)
            {
                return NotFound();
            }*/

            if (ModelState.IsValid)
            {
                var aircraft = await _context.Aircraft.FirstOrDefaultAsync(a => a.AircraftId == flight.AircraftId);
                if (aircraft == null)
                {
                    ModelState.AddModelError("", "Invalid aircraft.");
                    return View(flight);
                }

                if (flight.TotalSeats > aircraft.Capacity)
                {
                    ModelState.AddModelError("TotalSeats", "Total seats cannot exceed aircraft capacity.");
                    return View(flight);
                }

                if (flight.DepartureTime.AddMonths(1) < DateTime.Now)
                {
                    ModelState.AddModelError("DepartureTime", "Departure time must be at least one month from now.");
                    return View(flight);
                }

                if (flight.ArrivalTime <= flight.DepartureTime)
                {
                    ModelState.AddModelError("ArrivalTime", "Arrival time must be after departure time.");
                    return View(flight);
                }

                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.FlightId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Load aircraft and airport lists
            var aircraftList = await _context.Aircraft
                                            .Select(a => new SelectListItem
                                            {
                                                Value = a.AircraftId.ToString(),
                                                Text = a.AircraftName
                                            }).ToListAsync();
            var airportList = await _context.Airports
                                           .Select(a => new SelectListItem
                                           {
                                               Value = a.AirportId.ToString(),
                                               Text = a.AirportName
                                           }).ToListAsync();

            // Pass data to ViewBag
            ViewBag.AircraftList = aircraftList;
            ViewBag.AirporList = airportList;

            // Pass data to ViewData for form selection
            ViewData["AircraftId"] = new SelectList(_context.Aircraft, "AircraftId", "AircraftName", flight.AircraftId);
            ViewData["ArrivalAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportName", flight.ArrivalAirportId);
            ViewData["DepartureAirportId"] = new SelectList(_context.Airports, "AirportId", "AirportName", flight.DepartureAirportId);

            return View(flight);
        }


        // GET: Flights/Flights/Delete/5
        [Route("/delete-flight")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.DepartureAirport)
                .FirstOrDefaultAsync(m => m.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/delete-flight")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.FlightId == id);
        }
    }
}
