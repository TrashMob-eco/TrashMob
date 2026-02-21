namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/routes")]
    public class RouteMetadataController(IEventAttendeeRouteManager eventAttendeeRouteManager) : SecureController
    {
        private readonly IEventAttendeeRouteManager eventAttendeeRouteManager = eventAttendeeRouteManager;

        /// <summary>
        /// Updates route metadata (privacy, trim, notes). Owner only.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <param name="request">The metadata update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{routeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DisplayEventAttendeeRoute), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRouteMetadata(Guid routeId, UpdateRouteMetadataRequest request,
            CancellationToken cancellationToken)
        {
            var result = await eventAttendeeRouteManager
                .UpdateRouteMetadataAsync(routeId, UserId, request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage.Contains("not found"))
                {
                    return NotFound(result.ErrorMessage);
                }

                return BadRequest(result.ErrorMessage);
            }

            TrackEvent(nameof(UpdateRouteMetadata));
            return Ok(result.Data.ToDisplayEventAttendeeRoute());
        }
    }
}
