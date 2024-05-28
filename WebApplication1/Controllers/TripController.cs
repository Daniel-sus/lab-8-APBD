using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab8.Context;
using lab8.Models;

namespace lab8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TripController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTrips(int page = 1, int pageSize = 10)
        {
            var trips = await _context.Trips
                .OrderByDescending(t => t.DateFrom)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(trips);
        }

        [HttpDelete("clients/{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await _context.Clients.FindAsync(idClient);
            if (client == null)
            {
                return NotFound();
            }

            var hasAssignedTrips = await _context.Client_Trips
                .AnyAsync(ct => ct.IdClient == idClient);

            if (hasAssignedTrips)
            {
                return Conflict("Client has assigned trips.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientTripRequest request)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Pesel == request.Pesel);

            if (client != null)
            {
                return Conflict("Client with PESEL already exists.");
            }

            var isAlreadyAssigned = await _context.Client_Trips
                .AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClientNavigation.Pesel == request.Pesel);

            if (isAlreadyAssigned)
            {
                return Conflict("Client is already registered for the trip.");
            }

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.IdTrip == idTrip && t.DateFrom > DateTime.Now);

            if (trip == null)
            {
                return NotFound("Trip does not exist or has already occurred.");
            }

            var clientTrip = new Client_Trip
            {
                IdClient = client?.IdClient ?? 0,
                IdTrip = idTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = DateTime.Parse(request.PaymentDate)
            };

            _context.Client_Trips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}