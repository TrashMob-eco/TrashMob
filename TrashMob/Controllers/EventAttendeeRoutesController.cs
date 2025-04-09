namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventattendeeroutes")]
    public class EventAttendeeRoutesController(IEventAttendeeRouteManager eventAttendeeRouteManager) : SecureController
    {
        private readonly IEventAttendeeRouteManager eventAttendeeRouteManager = eventAttendeeRouteManager;

        [HttpGet("{eventId}/{userId}")]
        public async Task<IActionResult> GetEventAttendeeRoutes(Guid eventId, Guid userId)
        {
            var result = (await eventAttendeeRouteManager.GetByParentIdAsync(eventId, CancellationToken.None).ConfigureAwait(false)).Where(e => e.CreatedByUserId == userId);

            var displayEventAttendeeRoutes = result.Select(x => x.ToDisplayEventAttendeeRoute()).ToList();

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        [HttpGet("byeventid/{eventId}")]
        public async Task<IActionResult> GetEventAttendeeRoutesByEventId(Guid eventId)
        {
            var result = await eventAttendeeRouteManager.GetByParentIdAsync(eventId, CancellationToken.None).ConfigureAwait(false);

            var displayEventAttendeeRoutes = result.Select(x => x.ToDisplayEventAttendeeRoute()).ToList();

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(displayEventAttendeeRoutes);
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetEventAttendeeRoutesByUserId(Guid userId)
        {
            var result = await eventAttendeeRouteManager.GetByCreatedUserIdAsync(userId, CancellationToken.None).ConfigureAwait(false);

            var displayEventAttendeeRoutes = result.Select(x => x.ToDisplayEventAttendeeRoute()).ToList();

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventAttendeeRoute(Guid id)
        {
            var result = await eventAttendeeRouteManager.GetAsync(x => x.Id == id, CancellationToken.None).ConfigureAwait(false);

            if (result == null || result.Count() == 0)
            {
                return NotFound();
            }

            var displayEventAttendeeRoute = result.FirstOrDefault().ToDisplayEventAttendeeRoute();

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventAttendeeRoute(DisplayEventAttendeeRoute displayEventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, displayEventAttendeeRoute,
                AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var eventAttendeeRoute = displayEventAttendeeRoute.ToEventAttendeeRoute();

            var updatedEventAttendeeRoute = await eventAttendeeRouteManager
                .UpdateAsync(eventAttendeeRoute, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdateEventAttendeeRoute));

            return Ok(updatedEventAttendeeRoute);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventAttendeeRoute(DisplayEventAttendeeRoute displayEventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            var eventAttendeeRoute = displayEventAttendeeRoute.ToEventAttendeeRoute();

            await eventAttendeeRouteManager.AddAsync(eventAttendeeRoute, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventAttendeeRoute));
            return Ok();
        }

        [HttpDelete("{routeId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventAttendeeRoute(Guid routeId,
            CancellationToken cancellationToken)
        {
            await eventAttendeeRouteManager.DeleteAsync(routeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventAttendeeRoute));

            return new NoContentResult();
        }
    }
}