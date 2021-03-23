using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashMob.Models;
using TrashMob.Persistence;

namespace TrashMob.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MobEventsController : ControllerBase
    {
        private readonly IMobEventRepository mobEventRepository;

        public MobEventsController(IMobEventRepository mobEventRepository)
        {
            this.mobEventRepository = mobEventRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMobEvents()
        {
            var result = await mobEventRepository.GetAllMobEvents().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMobEvent(Guid id)
        {
            var mobEvent = await mobEventRepository.GetMobEvent(id).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        // PUT: api/MobEvents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMobEvent(Guid id, MobEvent mobEvent)
        {
            try
            {
                var updatedEvent = await mobEventRepository.UpdateMobEvent(mobEvent).ConfigureAwait(false);
                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MobEventExists(id).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/MobEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostMobEvent(MobEvent mobEvent)
        {
            var newEventId = await mobEventRepository.AddMobEvent(mobEvent).ConfigureAwait(false);

            return CreatedAtAction("GetMobEvent", new { id = newEventId }, mobEvent);
        }

        // DELETE: api/MobEvents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMobEvent(Guid id)
        {
            await mobEventRepository.DeleteMobEvent(id).ConfigureAwait(false);
            return NoContent();
        }

        private async Task<bool> MobEventExists(Guid id)
        {
            return (await mobEventRepository.GetAllMobEvents().ConfigureAwait(false)).Any(e => e.MobEventId == id);
        }
    }
}
