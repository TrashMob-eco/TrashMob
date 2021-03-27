
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Persistence;

    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventStatusRepository eventStatusRepository;
        private readonly IEventTypeRepository eventTypeRepository;

        public EventsController(IEventRepository eventRepository, IEventStatusRepository eventStatusRepository, IEventTypeRepository eventTypeRepository)
        {
            this.eventRepository = eventRepository;
            this.eventStatusRepository = eventStatusRepository;
            this.eventTypeRepository = eventTypeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var result = await eventRepository.GetAllEvents().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStatuses()
        {
            var result = await eventStatusRepository.GetAllEventStatuses().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventTypes()
        {
            var result = await eventTypeRepository.GetAllEventTypes().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var mobEvent = await eventRepository.GetEvent(id).ConfigureAwait(false);

            if (mobEvent == null)
            {
                return NotFound();
            }

            return Ok(mobEvent);
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, Event mobEvent)
        {
            try
            {
                var updatedEvent = await eventRepository.UpdateEvent(mobEvent).ConfigureAwait(false);
                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(id).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Events
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostEvent(Event mobEvent)
        {
            var newEventId = await eventRepository.AddEvent(mobEvent).ConfigureAwait(false);

            return CreatedAtAction("GetEvent", new { id = newEventId }, mobEvent);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await eventRepository.DeleteEvent(id).ConfigureAwait(false);
            return NoContent();
        }

        private async Task<bool> EventExists(Guid id)
        {
            return (await eventRepository.GetAllEvents().ConfigureAwait(false)).Any(e => e.Id == id);
        }
    }
}
