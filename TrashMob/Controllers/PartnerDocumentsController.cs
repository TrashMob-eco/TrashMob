namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : SecureController
    {
        private readonly IKeyedManager<PartnerDocument> manager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerDocumentsController(IKeyedManager<Partner> partnerManager,
                                          IKeyedManager<PartnerDocument> manager)
            : base()
        {
            this.manager = manager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerDocuments(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var documents = await manager.GetByParentId(partnerId, cancellationToken);
            return Ok(documents);
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
