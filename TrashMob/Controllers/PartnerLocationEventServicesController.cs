namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing partner location event services, including retrieval by location and user.
    /// </summary>
    [Route("api/partnerlocationeventservices")]
    public class PartnerLocationEventServicesController(
        IEventPartnerLocationServiceManager eventPartnerLocationServicesManager,
        IPartnerLocationManager partnerLocationManager)
        : SecureController
    {

        /// <summary>
        /// Gets all event services for a given partner location.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of event services for the partner location.</remarks>
        [HttpGet("{partnerLocationId}")]
        public async Task<IActionResult> GetPartnerLocationEventServices(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var events = await eventPartnerLocationServicesManager
                .GetByPartnerLocationAsync(partnerLocationId, cancellationToken);

            return Ok(events);
        }

        /// <summary>
        /// Gets all event services for a given user. Requires a valid user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of event services for the user.</remarks>
        [HttpGet("getbyuser/{UserId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetPartnerLocationEventServicesByUser(Guid userId,
            CancellationToken cancellationToken)
        {
            var events = await eventPartnerLocationServicesManager.GetByUserAsync(userId, cancellationToken);

            return Ok(events);
        }
    }
}