namespace TrashMob.Controllers.V2
{
    using System;
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
    /// V2 controller for team logo operations as a nested resource under teams.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams/{teamId}/logo")]
    public class TeamLogosV2Controller(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        IImageManager imageManager,
        ILogger<TeamLogosV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Uploads a logo for a team. Replaces any existing logo. Only team leads can upload logos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="imageUpload">The image upload data (multipart/form-data).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated team with the new logo URL.</response>
        /// <response code="400">Invalid upload data.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadLogo(
            Guid teamId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadTeamLogo Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Delete existing logo if present
            if (!string.IsNullOrWhiteSpace(team.LogoUrl))
            {
                await imageManager.DeleteImageAsync(teamId, ImageTypeEnum.TeamLogo);
            }

            // Upload new logo to blob storage
            imageUpload.ParentId = teamId;
            imageUpload.ImageType = ImageTypeEnum.TeamLogo;
            await imageManager.UploadImageAsync(imageUpload);

            // Get the image URL and update team
            var logoUrl = await imageManager.GetImageUrlAsync(teamId, ImageTypeEnum.TeamLogo, ImageSizeEnum.Reduced, cancellationToken);
            team.LogoUrl = logoUrl;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(team, UserId, cancellationToken);
            return Ok(updatedTeam.ToV2Dto());
        }

        /// <summary>
        /// Deletes the team logo. Only team leads can delete logos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Logo deleted.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found.</response>
        [HttpDelete]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLogo(
            Guid teamId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteTeamLogo Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Delete logo from blob storage
            if (!string.IsNullOrWhiteSpace(team.LogoUrl))
            {
                await imageManager.DeleteImageAsync(teamId, ImageTypeEnum.TeamLogo);
            }

            // Clear logo URL on team
            team.LogoUrl = string.Empty;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            await teamManager.UpdateAsync(team, UserId, cancellationToken);
            return NoContent();
        }
    }
}
