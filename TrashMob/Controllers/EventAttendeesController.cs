
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
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventattendees")]
    public class EventAttendeesController : SecureController
    {
        private readonly IEventAttendeeManager eventAttendeeManager;

        public EventAttendeesController(IEventAttendeeManager eventAttendeeManager) : base()
        {
            this.eventAttendeeManager = eventAttendeeManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventAttendees(Guid eventId)
        {            
            var result = (await eventAttendeeManager.GetByParentIdAsync(eventId, CancellationToken.None).ConfigureAwait(false)).Select(u => u.User.ToDisplayUser());
            TelemetryClient.TrackEvent(nameof(GetEventAttendees));
            return Ok(result);
        }

        [HttpPut("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventAttendee(EventAttendee eventAttendee, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventAttendee, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var updatedEventAttendee = await eventAttendeeManager.UpdateAsync(eventAttendee, UserId, cancellationToken).ConfigureAwait(false);
                TelemetryClient.TrackEvent(nameof(UpdateEventAttendee));

                return Ok(updatedEventAttendee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventAttendeeExists(eventAttendee.EventId, eventAttendee.UserId, cancellationToken).ConfigureAwait(false))
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
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventAttendee(EventAttendee eventAttendee, CancellationToken cancellationToken)
        {            
            await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventAttendee));
            return Ok();
        }

        [HttpDelete("{eventId}/{userId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            await eventAttendeeManager.Delete(eventId, userId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventAttendee));

            return new NoContentResult();
        }

        private async Task<bool> EventAttendeeExists(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            var attendee = await eventAttendeeManager.GetAsync(ea => ea.EventId == eventId && ea.UserId == userId, cancellationToken).ConfigureAwait(false);

            return attendee != null;
        }
    }
}
