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
    using TrashMob.Shared.Extensions.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for site admin operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class AdminV2Controller(
        IKeyedManager<PartnerRequest> partnerRequestManager,
        IEmailManager emailManager,
        ILogger<AdminV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Updates a partner request.
        /// </summary>
        /// <param name="userId">The user ID performing the update.</param>
        /// <param name="partnerRequestDto">The updated partner request data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated partner request.</returns>
        /// <response code="200">Partner request updated.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPut("partnerrequestupdate/{userId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdatePartnerRequest(
            Guid userId,
            [FromBody] PartnerRequestDto partnerRequestDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerRequest RequestId={RequestId} UserId={UserId}", partnerRequestDto.Id, userId);

            var entity = partnerRequestDto.ToEntity();
            var result = await partnerRequestManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Gets all email templates.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of email templates.</returns>
        /// <response code="200">Returns the email templates.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet("emailTemplates")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailTemplateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEmailTemplates(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEmailTemplates");

            var templates = await emailManager.GetEmailTemplatesAsync(cancellationToken);

            return Ok(templates.Select(t => t.ToV2Dto()));
        }
    }
}
