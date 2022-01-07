
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
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Poco;
    using System.Threading;

    [ApiController]
    [Route("api/eventattendees")]
    public class EventAttendeesController : ControllerBase
    {
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserRepository userRepository;

        public EventAttendeesController(IEventAttendeeRepository eventAttendeeRepository, IUserRepository userRepository)
        {
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventAttendees(Guid eventId, CancellationToken cancellationToken)
        {
            var result = (await eventAttendeeRepository.GetEventAttendees(eventId, cancellationToken).ConfigureAwait(false)).Select(u => u.ToDisplayUser());
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEventAttendee(EventAttendee eventAttendee)
        {
            var user = await userRepository.GetUserByInternalId(eventAttendee.UserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

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
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostEventAttendee(EventAttendee eventAttendee)
        {
            var user = await userRepository.GetUserByInternalId(eventAttendee.UserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventAttendeeRepository.AddEventAttendee(eventAttendee.EventId, eventAttendee.UserId).ConfigureAwait(false);

            return CreatedAtAction("GetEventAttendees", new { eventId = eventAttendee.EventId });
        }

        [HttpDelete("{eventId}/{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventAttendeeRepository.DeleteEventAttendee(eventId, userId).ConfigureAwait(false);
            return Ok();
        }

        private async Task<bool> EventAttendeeExists(Guid eventId, Guid userId)
        {
            return (await eventAttendeeRepository.GetEventAttendees(eventId).ConfigureAwait(false)).Any(e => e.Id == userId);
        }

        // Ensure the user calling in is the owner of the record
        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
