
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
    using TrashMob.Poco;
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/eventattendees")]
    public class EventAttendeesController : BaseController
    {
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserRepository userRepository;

        public EventAttendeesController(IEventAttendeeRepository eventAttendeeRepository,
                                        IUserRepository userRepository,
                                        TelemetryClient telemetryClient) 
            : base(telemetryClient)
        {
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventAttendees(Guid eventId)
        {
            
            var result = (await eventAttendeeRepository.GetEventAttendees(eventId, CancellationToken.None).ConfigureAwait(false)).Select(u => u.ToDisplayUser());
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventAttendee(EventAttendee eventAttendee)
        {
            var user = await userRepository.GetUserByInternalId(eventAttendee.UserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                var updatedEventAttendee = await eventAttendeeRepository.UpdateEventAttendee(eventAttendee).ConfigureAwait(false);
                TelemetryClient.TrackEvent(nameof(UpdateEventAttendee));

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
        public async Task<IActionResult> AddEventAttendee(EventAttendee eventAttendee)
        {
            var user = await userRepository.GetUserByInternalId(eventAttendee.UserId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventAttendeeRepository.AddEventAttendee(eventAttendee.EventId, eventAttendee.UserId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventAttendee));

            var result = (await eventAttendeeRepository.GetEventAttendees(eventAttendee.EventId, CancellationToken.None).ConfigureAwait(false)).Select(u => u.ToDisplayUser());
            return Ok(result);
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
            TelemetryClient.TrackEvent(nameof(DeleteEventAttendee));

            return Ok();
        }

        private async Task<bool> EventAttendeeExists(Guid eventId, Guid userId)
        {
            return (await eventAttendeeRepository.GetEventAttendees(eventId).ConfigureAwait(false)).Any(e => e.Id == userId);
        }
    }
}
