namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerlocationeventservices")]
    public class PartnerLocationEventServicesController : SecureController
    {
        private readonly IEventPartnerLocationServiceManager eventPartnerLocationServicesManager;
        private readonly IPartnerLocationManager partnerLocationManager;

        public PartnerLocationEventServicesController(
            IEventPartnerLocationServiceManager eventPartnerLocationServicesManager,
            IPartnerLocationManager partnerLocationManager)
        {
            this.eventPartnerLocationServicesManager = eventPartnerLocationServicesManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        [HttpGet("{partnerLocationId}")]
        public async Task<IActionResult> GetPartnerLocationEventServices(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var events = await eventPartnerLocationServicesManager
                .GetByPartnerLocationAsync(partnerLocationId, cancellationToken).ConfigureAwait(false);

            return Ok(events);
        }

        [HttpGet("getbyuser/{UserId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetPartnerLocationEventServicesByUser(Guid userId,
            CancellationToken cancellationToken)
        {
            var events = await eventPartnerLocationServicesManager.GetByUserAsync(userId, cancellationToken)
                .ConfigureAwait(false);

            return Ok(events);
        }
    }
}