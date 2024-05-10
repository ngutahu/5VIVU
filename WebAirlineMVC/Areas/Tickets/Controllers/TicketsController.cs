using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAirlineMVC.Models;

namespace WebAirlineMVC.Areas.Tickets.Controllers
{
    [Area("Tickets")]
    public class TicketsController : Controller
    {
        private readonly VietJetContext _context;

        public TicketsController(VietJetContext context)
        {
            _context = context;
        }        
        public async Task<IActionResult> Index()
        {
            var vietJetContext = _context.Tickets.Include(t => t.SoldSeat);
            return View(await vietJetContext.ToListAsync());
        }        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.SoldSeat)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Tickets/Create
        public IActionResult Create()
        {
            ViewData["SoldSeatId"] = new SelectList(_context.SoldSeats, "SoldSeatId", "SoldSeatId");
            return View();
        }

        // POST: Tickets/Tickets/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,SoldSeatId,SeatNumber,Price,IsBooked,FullName,IdentificationNumber")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoldSeatId"] = new SelectList(_context.SoldSeats, "SoldSeatId", "SoldSeatId", ticket.SoldSeatId);
            return View(ticket);
        }

        // GET: Tickets/Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["SoldSeatId"] = new SelectList(_context.SoldSeats, "SoldSeatId", "SoldSeatId", ticket.SoldSeatId);
            return View(ticket);
        }

        // POST: Tickets/Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,SoldSeatId,SeatNumber,Price,IsBooked,FullName,IdentificationNumber")] Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.TicketId))
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
            ViewData["SoldSeatId"] = new SelectList(_context.SoldSeats, "SoldSeatId", "SoldSeatId", ticket.SoldSeatId);
            return View(ticket);
        }

        // GET: Tickets/Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.SoldSeat)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}
