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

    /// <summary>
    /// Controller for managing partner documents, including retrieval and creation.
    /// </summary>
    [Authorize]
    [Route("api/partnerdocuments")]
    public class PartnerDocumentsController : SecureController
    {
        private readonly IPartnerDocumentManager manager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDocumentsController"/> class.
        /// </summary>
        /// <param name="partnerManager">The partner manager.</param>
        /// <param name="manager">The partner document manager.</param>
        public PartnerDocumentsController(IKeyedManager<Partner> partnerManager,
            IPartnerDocumentManager manager)
        {
            this.manager = manager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all documents for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner documents.</remarks>
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

        /// <summary>
        /// Gets a partner document by its unique identifier. Requires a valid user.
        /// </summary>
        /// <param name="partnerDocumentId">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner document.</remarks>
        [HttpGet("{partnerDocumentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partnerDocument = await manager.GetAsync(partnerDocumentId, cancellationToken);

            return Ok(partnerDocument);
        }

        /// <summary>
        /// Adds a new partner document.
        /// </summary>
        /// <param name="partnerDocument">The partner document to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
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

        /// <summary>
        /// Updates an existing partner document.
        /// </summary>
        /// <param name="partnerDocument">The partner document to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
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

        /// <summary>
        /// Deletes a partner document by its unique identifier.
        /// </summary>
        /// <param name="partnerDocumentId">The partner document ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
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