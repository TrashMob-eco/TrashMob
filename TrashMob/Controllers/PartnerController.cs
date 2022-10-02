namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partners")]
    public class PartnersController : KeyedController<Partner>
    {
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnersController(TelemetryClient telemetryClient,
                                  IAuthorizationService authorizationService,
                                  IKeyedManager<Partner> partnerManager) 
            : base(telemetryClient, authorizationService, partnerManager)
        {
            this.partnerManager = partnerManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> Get(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(await partnerManager.Get(partnerId, cancellationToken).ConfigureAwait(false));
        }

        [HttpPut]
        public async Task<IActionResult> Update(Partner partner)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerManager.Update(partner).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Update) + typeof(Partner));

            return Ok(result);
        }
    }
}
