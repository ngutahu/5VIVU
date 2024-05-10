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
    public class AirportsController : ControllerBase
    {
        private readonly VietJetContext _context;

        public AirportsController(VietJetContext context)
        {
            _context = context;
        }

        // GET: api/Airports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAirports()
        {
            var airPorts= await _context.Airports
                            .Select(a => new
                            {
                                airportId = a.AirportId,
                                airportName = a.AirportName,
                                location = a.Location
                            })
                            .ToListAsync();
            
            return airPorts;
        }

        // GET: api/Airports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAirport(int id)
        {
            var airport = await _context.Airports
                          .Where(a=>a.AirportId==id)
                          .Select(a => new
                          {
                              airportId = a.AirportId,
                              airportName = a.AirportName,
                              location = a.Location
                          }).FirstOrDefaultAsync();           

            if (airport == null)
            {
                return NotFound();
            }

            return airport;
        }      
    }
}
