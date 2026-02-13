namespace TrashMob.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/users")]
    public class UserRoutesController(IEventAttendeeRouteManager eventAttendeeRouteManager) : SecureController
    {
        private readonly IEventAttendeeRouteManager eventAttendeeRouteManager = eventAttendeeRouteManager;

        /// <summary>
        /// Gets the current user's route history.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("me/routes")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<DisplayUserRouteHistory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyRoutes(CancellationToken cancellationToken)
        {
            var routes = await eventAttendeeRouteManager
                .GetUserRouteHistoryAsync(UserId, cancellationToken);

            TrackEvent(nameof(GetMyRoutes));
            return Ok(routes);
        }
    }
}
