namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : SecureController
    {
        private readonly IPartnerDocumentManager manager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerDocumentsController(IKeyedManager<Partner> partnerManager,
            IPartnerDocumentManager manager)
        {
            this.manager = manager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerDocuments(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var documents = await manager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(documents);
        }

        [HttpGet("{partnerDocumentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partnerDocument = await manager.GetAsync(partnerDocumentId, cancellationToken);

            return Ok(partnerDocument);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PartnerDocument partnerDocument, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerDocument.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.AddAsync(partnerDocument, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Add) + typeof(PartnerDocument));

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(PartnerDocument partnerDocument, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerDocument.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await manager.UpdateAsync(partnerDocument, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Update) + typeof(PartnerDocument));

            return Ok(result);
        }

        [HttpDelete("{partnerDocumentId}")]
        public async Task<IActionResult> Delete(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partner = await manager.GetPartnerForDocument(partnerDocumentId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.DeleteAsync(partnerDocumentId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Delete) + typeof(PartnerDocument));

            return Ok(partnerDocumentId);
        }
    }
}