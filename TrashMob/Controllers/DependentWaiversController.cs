namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing dependent waivers.
    /// </summary>
    [Route("api/dependents/{dependentId}/waiver")]
    public class DependentWaiversController(
        IDependentWaiverManager dependentWaiverManager,
        IDependentManager dependentManager,
        IWaiverDocumentManager waiverDocumentManager,
        IUserManager userManager)
        : SecureController
    {
        /// <summary>
        /// Signs a waiver on behalf of a dependent.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="request">The waiver signing request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created dependent waiver.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentWaiver), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SignWaiver(
            Guid dependentId, SignDependentWaiverRequest request, CancellationToken cancellationToken)
        {
            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != UserId)
            {
                return Forbid();
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await dependentWaiverManager.SignWaiverAsync(
                dependentId,
                request.WaiverVersionId,
                request.TypedLegalName,
                ipAddress,
                userAgent,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            // Generate and store PDF
            try
            {
                var user = await userManager.GetAsync(UserId, cancellationToken);
                var documentUrl = await waiverDocumentManager
                    .GenerateAndStoreDependentWaiverPdfAsync(result.Data, dependent, user, cancellationToken);
                result.Data.DocumentUrl = documentUrl;
                await dependentWaiverManager.UpdateAsync(result.Data, cancellationToken);
            }
            catch (Exception ex)
            {
                // Don't fail waiver acceptance if PDF generation fails
                Logger?.LogWarning(ex, "PDF generation failed for dependent waiver {WaiverId}", result.Data.Id);
            }

            TrackEvent(nameof(SignWaiver));
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        /// <summary>
        /// Gets the current valid waiver for a dependent.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The current waiver or 404 if none.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(DependentWaiver), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentWaiver(Guid dependentId, CancellationToken cancellationToken)
        {
            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != UserId)
            {
                return Forbid();
            }

            var waiver = await dependentWaiverManager.GetCurrentWaiverAsync(dependentId, cancellationToken);
            if (waiver == null)
            {
                return NotFound();
            }

            return Ok(waiver);
        }

        /// <summary>
        /// Gets the waiver history for a dependent.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all waivers signed for this dependent.</returns>
        [HttpGet("history")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<DependentWaiver>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetWaiverHistory(Guid dependentId, CancellationToken cancellationToken)
        {
            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != UserId)
            {
                return Forbid();
            }

            var waivers = await dependentWaiverManager.GetByDependentIdAsync(dependentId, cancellationToken);
            return Ok(waivers);
        }

        /// <summary>
        /// Downloads the PDF for a signed dependent waiver.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="waiverId">The dependent waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The waiver PDF file.</returns>
        [HttpGet("{waiverId:guid}/download")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadWaiverPdf(
            Guid dependentId, Guid waiverId, CancellationToken cancellationToken)
        {
            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != UserId)
            {
                return Forbid();
            }

            var waiver = await dependentWaiverManager.GetAsync(waiverId, cancellationToken);
            if (waiver == null || waiver.DependentId != dependentId)
            {
                return NotFound();
            }

            // If no stored document, generate one on-the-fly
            if (string.IsNullOrWhiteSpace(waiver.DocumentUrl))
            {
                var user = await userManager.GetAsync(UserId, cancellationToken);
                var pdfBytes = waiverDocumentManager.GenerateDependentWaiverPdf(waiver, dependent, user);

                TrackEvent(nameof(DownloadWaiverPdf));

                return File(pdfBytes, "application/pdf", $"dependent-waiver-{waiver.Id}.pdf");
            }

            TrackEvent(nameof(DownloadWaiverPdf));

            return Redirect(waiver.DocumentUrl);
        }

        /// <summary>
        /// Request model for signing a dependent waiver.
        /// </summary>
        public class SignDependentWaiverRequest
        {
            /// <summary>
            /// Gets or sets the waiver version to sign.
            /// </summary>
            public Guid WaiverVersionId { get; set; }

            /// <summary>
            /// Gets or sets the typed legal name of the signer.
            /// </summary>
            public string TypedLegalName { get; set; }
        }
    }
}
