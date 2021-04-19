
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Common;
    using TrashMob.Models;
    using TrashMob.Persistence;

    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserRepository userRepository;

        public EventsController(IEventRepository eventRepository, IEventAttendeeRepository eventAttendeeRepository, IUserRepository userRepository)
        {
            this.eventRepository = eventRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var result = await eventRepository.GetAllEvents().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveEvents()
        {
            var result = await eventRepository.GetActiveEvents().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("eventsowned/{userId}")]
        public async Task<IActionResult> GetEventsUserOwns(Guid userId)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);
            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result = await eventRepository.GetUserEvents(userId).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("eventsuserisattending/{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);
            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result = await eventAttendeeRepository.GetEventsUserIsAttending(userId).ConfigureAwait(false);
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
        [HttpPut]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEvent(Event mobEvent)
        {
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);
            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                var updatedEvent = await eventRepository.UpdateEvent(mobEvent).ConfigureAwait(false);
                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(mobEvent.Id).ConfigureAwait(false))
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
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostEvent(Event mobEvent)
        {
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);
            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventRepository.AddEvent(mobEvent).ConfigureAwait(false);

            return Ok();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var mobEvent = await eventRepository.GetEvent(id);
            var user = await userRepository.GetUserByInternalId(mobEvent.CreatedByUserId).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventRepository.DeleteEvent(id).ConfigureAwait(false);
            return NoContent();
        }

        private async Task<bool> EventExists(Guid id)
        {
            return (await eventRepository.GetAllEvents().ConfigureAwait(false)).Any(e => e.Id == id);
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
