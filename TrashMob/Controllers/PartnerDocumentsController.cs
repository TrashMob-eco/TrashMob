namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : SecureController
    {
        private readonly IBaseManager<PartnerDocument> manager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerDocumentsController(TelemetryClient telemetryClient,
                                          IAuthorizationService authorizationService,
                                          IKeyedManager<Partner> partnerManager,
                                          IBaseManager<PartnerDocument> manager)
            : base(telemetryClient, authorizationService)
        {
            this.manager = manager;
            this.partnerManager = partnerManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(PartnerDocument partnerDocument)
        {
            var partner = partnerManager.Get(partnerDocument.PartnerId);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.Add(partnerDocument, UserId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Add) + typeof(PartnerDocument));

            return Ok();
        }
    }
}
