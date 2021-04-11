
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Persistence;

    [ApiController]
    [Route("api/eventattendees")]
    public class EventAttendeesController : ControllerBase
    {
        private readonly IEventAttendeeRepository eventAttendeeRepository;

        public EventAttendeesController(IEventAttendeeRepository eventAttendeeRepository)
        {
            this.eventAttendeeRepository = eventAttendeeRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventAttendees(Guid eventId)
        {
            var result = await eventAttendeeRepository.GetEventAttendees(eventId).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventAttendee(EventAttendee eventAttendee)
        {
            try
            {
                var updatedEventAttendee = await eventAttendeeRepository.UpdateEventAttendee(eventAttendee).ConfigureAwait(false);
                return Ok(updatedEventAttendee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventAttendeeExists(eventAttendee.EventId, eventAttendee.UserId).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostEventAttendee(EventAttendee eventAttendee)
        {
            await eventAttendeeRepository.AddEventAttendee(eventAttendee.EventId, eventAttendee.UserId).ConfigureAwait(false);

            return CreatedAtAction("GetEventAttendees", new { eventId = eventAttendee.EventId });
        }

        [HttpDelete("{eventId}/{userId}")]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId)
        {
            await eventAttendeeRepository.DeleteEventAttendee(eventId, userId).ConfigureAwait(false);
            return NoContent();
        }

        private async Task<bool> EventAttendeeExists(Guid eventId, Guid userId)
        {
            return (await eventAttendeeRepository.GetEventAttendees(eventId).ConfigureAwait(false)).Any(e => e.Id == userId);
        }
    }
}
