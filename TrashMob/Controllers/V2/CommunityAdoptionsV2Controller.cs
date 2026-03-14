namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for community admin adoption management.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{partnerId}/adoptions")]
    public class CommunityAdoptionsV2Controller(
        ITeamAdoptionManager adoptionManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<CommunityAdoptionsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all pending adoption applications for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("pending")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPendingApplications(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPendingApplications for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var applications = await adoptionManager.GetPendingByCommunityAsync(partnerId, cancellationToken);
            return Ok(applications.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Gets all approved adoptions for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("approved")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetApprovedAdoptions(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetApprovedAdoptions for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetApprovedByCommunityAsync(partnerId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Approves an adoption application. Only community admins can approve.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="adoptionId">The adoption application ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{adoptionId}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveApplication(
            Guid partnerId,
            Guid adoptionId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ApproveApplication Partner={PartnerId}, Adoption={AdoptionId}", partnerId, adoptionId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await adoptionManager.ApproveApplicationAsync(adoptionId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data.ToV2Dto());
        }

        /// <summary>
        /// Rejects an adoption application. Only community admins can reject.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="adoptionId">The adoption application ID.</param>
        /// <param name="request">The rejection request with reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{adoptionId}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectApplication(
            Guid partnerId,
            Guid adoptionId,
            [FromBody] RejectAdoptionRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RejectApplication Partner={PartnerId}, Adoption={AdoptionId}", partnerId, adoptionId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await adoptionManager.RejectApplicationAsync(
                adoptionId,
                request.RejectionReason,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data.ToV2Dto());
        }

        /// <summary>
        /// Gets delinquent adoptions for a community (teams not meeting cleanup requirements).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("delinquent")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDelinquentAdoptions(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetDelinquentAdoptions for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetDelinquentByCommunityAsync(partnerId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Gets compliance statistics for a community's adoption program.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("stats")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(AdoptionComplianceStats), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComplianceStats(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetComplianceStats for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var stats = await adoptionManager.GetComplianceStatsByCommunityAsync(partnerId, cancellationToken);
            return Ok(stats);
        }

        /// <summary>
        /// Exports all adoptions for a community in CSV format for signage updates.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("export")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportAdoptions(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ExportAdoptions for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetAllForExportByCommunityAsync(partnerId, cancellationToken);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Area Name,Area Type,Team Name,Status,Compliant,Event Count,Last Event Date,Adoption Start,Adoption End,Safety Requirements");

            foreach (var adoption in adoptions)
            {
                var areaName = EscapeCsvField(adoption.AdoptableArea?.Name ?? "");
                var areaType = EscapeCsvField(adoption.AdoptableArea?.AreaType ?? "");
                var teamName = EscapeCsvField(adoption.Team?.Name ?? "");
                var status = adoption.Status;
                var isCompliant = adoption.IsCompliant ? "Yes" : "No";
                var eventCount = adoption.EventCount.ToString();
                var lastEventDate = adoption.LastEventDate?.ToString("yyyy-MM-dd") ?? "";
                var startDate = adoption.AdoptionStartDate?.ToString("yyyy-MM-dd") ?? "";
                var endDate = adoption.AdoptionEndDate?.ToString("yyyy-MM-dd") ?? "";
                var safetyReqs = EscapeCsvField(adoption.AdoptableArea?.SafetyRequirements ?? "");

                csv.AppendLine($"{areaName},{areaType},{teamName},{status},{isCompliant},{eventCount},{lastEventDate},{startDate},{endDate},{safetyReqs}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"{partner.Name.Replace(" ", "_")}_Adoptions_{System.DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return "";
            }

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
