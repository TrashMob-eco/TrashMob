
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared;
    using TrashMob.Poco;
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;

    [Route("api/eventattendees")]
    public class EventAttendeesController : SecureController
    {
        private readonly IEventAttendeeRepository eventAttendeeRepository;

        public EventAttendeesController(TelemetryClient telemetryClient,
                                        IAuthorizationService authorizationService,
                                        IEventAttendeeRepository eventAttendeeRepository) 
            : base(telemetryClient, authorizationService)
        {
            this.eventAttendeeRepository = eventAttendeeRepository;
        }

        [HttpGet("{eventId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetEventAttendees(Guid eventId)
        {            
            var result = (await eventAttendeeRepository.GetEventAttendees(eventId, CancellationToken.None).ConfigureAwait(false)).Select(u => u.ToDisplayUser());
            return Ok(result);
        }

        [HttpPut("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventAttendee(EventAttendee eventAttendee)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventAttendee, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
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
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventAttendee(EventAttendee eventAttendee)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventAttendee, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await eventAttendeeRepository.AddEventAttendee(eventAttendee.EventId, eventAttendee.UserId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventAttendee));

            var result = (await eventAttendeeRepository.GetEventAttendees(eventAttendee.EventId, CancellationToken.None).ConfigureAwait(false)).Select(u => u.ToDisplayUser());
            return Ok(result);
        }

        [HttpDelete("{eventId}/{userId}")]
        // Todo: Tighten this down
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId)
        {
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
