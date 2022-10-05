namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partners")]
    public class PartnersController : KeyedController<Partner>
    {
        public PartnersController(IKeyedManager<Partner> partnerManager) 
            : base(partnerManager)
        {
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> Get(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetAsync(partnerId, cancellationToken).ConfigureAwait(false));
        }

        [HttpPut]
        public async Task<IActionResult> Update(Partner partner, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await Manager.UpdateAsync(partner, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Update) + typeof(Partner));

            return Ok(result);
        }
    }
}
