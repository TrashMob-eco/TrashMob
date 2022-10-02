namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnercontacts")]
    public class PartnerContactsController : SecureController
    {
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IBaseManager<PartnerContact> manager;

        public PartnerContactsController(TelemetryClient telemetryClient,
                                         IKeyedManager<Partner> partnerManager,
                                         IAuthorizationService authorizationService,
                                         IBaseManager<PartnerContact> manager)
            : base(telemetryClient, authorizationService)
        {
            this.partnerManager = partnerManager;
            this.manager = manager;            
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerContact(PartnerContact partnerContact)
        {
            var partner = partnerManager.Get(partnerContact.PartnerId);
            
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.Add(partnerContact, UserId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerContact));

            return Ok();
        }
    }
}
