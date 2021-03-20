using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashMob.Models;
using TrashMob.Persistence;

namespace TrashMob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobEventsController : ControllerBase
    {
        private readonly MobDbContext _context;

        public MobEventsController(MobDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobEvent>>> GetMobEvents()
        {
            return await _context.MobEvents.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MobEvent>> GetMobEvent(long id)
        {
            var mobEvent = await _context.MobEvents.FindAsync(id);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return mobEvent;
        }

        // PUT: api/MobEvents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMobEvent(Guid id, MobEvent mobEvent)
        {
            if (id != mobEvent.MobEventId)
            {
                return BadRequest();
            }

            _context.Entry(mobEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobEventExists(id))
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

        // POST: api/MobEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MobEvent>> PostMobEvent(MobEvent mobEvent)
        {
            _context.MobEvents.Add(mobEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMobEvent", new { id = mobEvent.MobEventId }, mobEvent);
        }

        // DELETE: api/MobEvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MobEvent>> DeleteMobEvent(long id)
        {
            var mobEvent = await _context.MobEvents.FindAsync(id);
            if (mobEvent == null)
            {
                return NotFound();
            }

            _context.MobEvents.Remove(mobEvent);
            await _context.SaveChangesAsync();

            return mobEvent;
        }

        private bool MobEventExists(Guid id)
        {
            return _context.MobEvents.Any(e => e.MobEventId == id);
        }
    }
}
