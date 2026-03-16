namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing partner location event services.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partnerlocationeventservices")]
    public class PartnerLocationEventServicesV2Controller(
        IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
        IPartnerLocationManager partnerLocationManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerLocationEventServicesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all event services for a given partner location.
        /// </summary>
        /// <param name="locationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the event services for the partner location.</response>
        /// <response code="403">Not authorized.</response>
        [HttpGet("{locationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayPartnerLocationServiceEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByPartnerLocation(Guid locationId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationEventServices PartnerLocation={PartnerLocationId}", locationId);

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(locationId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var events = await eventPartnerLocationServiceManager
                .GetByPartnerLocationAsync(locationId, cancellationToken);

            return Ok(events);
        }

        /// <summary>
        /// Gets all event services for a given user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the event services for the user.</response>
        [HttpGet("by-user/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayPartnerLocationServiceEvent>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationEventServicesByUser User={UserId}", userId);

            var events = await eventPartnerLocationServiceManager.GetByUserAsync(userId, cancellationToken);

            return Ok(events);
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
