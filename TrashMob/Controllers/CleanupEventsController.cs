using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashMob.Models;
using TrashMob.Persistence;

namespace TrashMob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CleanupEventsController : ControllerBase
    {
        private readonly MobDbContext _context;

        public CleanupEventsController(MobDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CleanupEvent>>> GetCleanupEvents()
        {
            return await _context.CleanupEvents.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CleanupEvent>> GetCleanupEvent(long id)
        {
            var cleanupEvent = await _context.CleanupEvents.FindAsync(id);

            if (cleanupEvent == null)
            {
                return NotFound();
            }

            return cleanupEvent;
        }

        // PUT: api/CleanupEvents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCleanupEvent(long id, CleanupEvent cleanupEvent)
        {
            if (id != cleanupEvent.CleanupEventId)
            {
                return BadRequest();
            }

            _context.Entry(cleanupEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CleanupEventExists(id))
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

        // POST: api/CleanupEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CleanupEvent>> PostCleanupEvent(CleanupEvent cleanupEvent)
        {
            _context.CleanupEvents.Add(cleanupEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCleanupEvent", new { id = cleanupEvent.CleanupEventId }, cleanupEvent);
        }

        // DELETE: api/CleanupEvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CleanupEvent>> DeleteCleanupEvent(long id)
        {
            var cleanupEvent = await _context.CleanupEvents.FindAsync(id);
            if (cleanupEvent == null)
            {
                return NotFound();
            }

            _context.CleanupEvents.Remove(cleanupEvent);
            await _context.SaveChangesAsync();

            return cleanupEvent;
        }

        private bool CleanupEventExists(long id)
        {
            return _context.CleanupEvents.Any(e => e.CleanupEventId == id);
        }
    }
}
