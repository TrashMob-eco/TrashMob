namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/eventattendeeroutes")]
    public class EventAttendeeRoutesController : SecureController
    {
        private readonly IEventAttendeeRouteManager eventAttendeeRouteManager;

        public EventAttendeeRoutesController(IEventAttendeeRouteManager eventAttendeeRouteManager)
        {
            this.eventAttendeeRouteManager = eventAttendeeRouteManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventAttendeeRoutes(Guid eventId)
        {
            var result =
                (await eventAttendeeRouteManager.GetByParentIdAsync(eventId, CancellationToken.None)
                    .ConfigureAwait(false)).Select(u => u.User.ToDisplayUser());
            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        [HttpPut("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventAttendeeRoute(EventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, eventAttendeeRoute,
                AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedEventAttendeeRoute = await eventAttendeeRouteManager
                .UpdateAsync(eventAttendeeRoute, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdateEventAttendeeRoute));

            return Ok(updatedEventAttendeeRoute);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventAttendeeRoute(EventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            await eventAttendeeRouteManager.AddAsync(eventAttendeeRoute, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventAttendeeRoute));
            return Ok();
        }

        [HttpDelete("{eventId}/{userId}/{routeId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventAttendeeRoute(Guid eventId, Guid userId, Guid routeId,
            CancellationToken cancellationToken)
        {
            await eventAttendeeRouteManager.DeleteAsync(routeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventAttendeeRoute));

            return new NoContentResult();
        }
    }
}