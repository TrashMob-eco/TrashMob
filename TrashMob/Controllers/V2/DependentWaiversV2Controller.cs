namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using System.Linq;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing dependent waivers.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/dependents/{dependentId}/waiver")]
    public class DependentWaiversV2Controller(
        IDependentWaiverManager dependentWaiverManager,
        IDependentManager dependentManager,
        IWaiverDocumentManager waiverDocumentManager,
        IUserManager userManager,
        ILogger<DependentWaiversV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets the current valid waiver for a dependent.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the current waiver.</response>
        /// <response code="403">User is not the parent.</response>
        /// <response code="404">No valid waiver found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(DependentWaiverDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentWaiver(Guid dependentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCurrentWaiver Dependent={DependentId}", dependentId);

            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != UserId)
            {
                return Forbid();
            }

            var waiver = await dependentWaiverManager.GetCurrentWaiverAsync(dependentId, cancellationToken);
            if (waiver == null)
            {
                return Problem(detail: $"No valid waiver found for dependent {dependentId}.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            return Ok(waiver.ToV2Dto());
        }

        /// <summary>
        /// Signs a waiver on behalf of a dependent.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="request">The waiver signing request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Waiver signed.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="403">User is not the parent.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentWaiverDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SignWaiver(
            Guid dependentId, SignDependentWaiverRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SignWaiver Dependent={DependentId}", dependentId);

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
                return Problem(detail: result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest, title: "Waiver signing failed");
            }

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
                logger.LogWarning(ex, "PDF generation failed for dependent waiver {WaiverId}", result.Data.Id);
            }

            return StatusCode(StatusCodes.Status201Created, result.Data.ToV2Dto());
        }

        /// <summary>
        /// Gets the waiver history for a dependent.
        /// </summary>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the waiver history.</response>
        /// <response code="403">User is not the parent.</response>
        [HttpGet("history")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<DependentWaiverDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetWaiverHistory(Guid dependentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetWaiverHistory Dependent={DependentId}", dependentId);

            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != UserId)
            {
                return Forbid();
            }

            var waivers = await dependentWaiverManager.GetByDependentIdAsync(dependentId, cancellationToken);
            return Ok(waivers.Select(w => w.ToV2Dto()));
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
