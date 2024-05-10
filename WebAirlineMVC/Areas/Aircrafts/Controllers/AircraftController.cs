using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAirlineMVC.Models;

namespace WebAirlineMVC.Areas.Aircrafts.Controllers
{
    [Area("Aircrafts")]
    public class AircraftController : Controller
    {
        private readonly VietJetContext _context;

        public AircraftController(VietJetContext context)
        {
            _context = context;
        }
        [Route("aircraft-list")]
        // GET: Aircrafts/Aircraft
        public async Task<IActionResult> Index()
        {
            return View(await _context.Aircraft.ToListAsync());
        }

        // GET: Aircrafts/Aircraft/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aircraft = await _context.Aircraft
                .FirstOrDefaultAsync(m => m.AircraftId == id);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }
        [Route("create-aircraft")]
        // GET: Aircrafts/Aircraft/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aircrafts/Aircraft/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("create-aircraft")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AircraftId,AircraftName")] Aircraft aircraft, [Bind("seatRow")] int seatRow, [Bind("seatColumn")] int seatColumn)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (seatColumn <= 0 || seatRow<=0)
                    {
                        ModelState.AddModelError("Aircraft Capacity", "Capacity must be greater than zero.");
                        return View(aircraft);
                    }
                    aircraft.Capacity= seatRow * seatColumn;

                    _context.Add(aircraft);
                    await _context.SaveChangesAsync();

                    char[] rows = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); 
                    for (int i = 0; i < seatRow; i++)
                    {
                        for (int j = 0; j < seatColumn; j++)
                        {
                            var seat = new Seat
                            {
                                AircraftId = aircraft.AircraftId,
                                SeatNumber = rows[i].ToString()+" "+ (j + 1).ToString() 
                            };
                            _context.Add(seat);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(aircraft);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(aircraft);
            }
        }

        // GET: Aircrafts/Aircraft/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aircraft = await _context.Aircraft.FindAsync(id);
            if (aircraft == null)
            {
                return NotFound();
            }
            return View(aircraft);
        }

        // POST: Aircrafts/Aircraft/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AircraftId,AircraftName,Capacity")] Aircraft aircraft)
        {
            if (id != aircraft.AircraftId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aircraft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AircraftExists(aircraft.AircraftId))
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
            return View(aircraft);
        }

        // GET: Aircrafts/Aircraft/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aircraft = await _context.Aircraft
                .FirstOrDefaultAsync(m => m.AircraftId == id);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }

        // POST: Aircrafts/Aircraft/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aircraft = await _context.Aircraft.FindAsync(id);
            if (aircraft != null)
            {
                _context.Aircraft.Remove(aircraft);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AircraftExists(int id)
        {
            return _context.Aircraft.Any(e => e.AircraftId == id);
        }
    }
}
