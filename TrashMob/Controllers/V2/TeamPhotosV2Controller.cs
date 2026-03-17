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
    /// V2 controller for team photo gallery operations as a nested resource under teams.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams/{teamId}/photos")]
    public class TeamPhotosV2Controller(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        ITeamPhotoManager teamPhotoManager,
        IImageManager imageManager,
        ILogger<TeamPhotosV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all photos for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the team photos.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TeamPhotoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPhotos(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetTeamPhotos Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            // Private teams can only have photos viewed by members
            if (!team.IsPublic)
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
                }
            }

            var photos = await teamPhotoManager.GetByTeamIdAsync(teamId, cancellationToken);
            return Ok(photos.Select(p => p.ToV2Dto()));
        }

        /// <summary>
        /// Uploads a photo for a team. Only team leads can upload photos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="imageUpload">The image upload data (multipart/form-data).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Photo uploaded successfully.</response>
        /// <response code="400">Invalid upload data.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamPhotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadPhoto(
            Guid teamId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadTeamPhoto Team={TeamId}", teamId);

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

            // Create the team photo record
            var photoId = Guid.NewGuid();
            var teamPhoto = new TeamPhoto
            {
                Id = photoId,
                TeamId = teamId,
                Caption = string.Empty,
            };

            // Upload to blob storage
            imageUpload.ParentId = photoId;
            imageUpload.ImageType = ImageTypeEnum.TeamPhoto;
            await imageManager.UploadImageAsync(imageUpload);

            // Get the image URL and save photo record
            var imageUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.TeamPhoto, ImageSizeEnum.Reduced, cancellationToken);
            teamPhoto.ImageUrl = imageUrl;
            var createdPhoto = await teamPhotoManager.AddAsync(teamPhoto, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetPhotos), new { teamId }, createdPhoto.ToV2Dto());
        }

        /// <summary>
        /// Updates a team photo caption. Only team leads can update photos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="caption">The new caption text.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated photo.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or photo not found.</response>
        [HttpPut("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamPhotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePhotoCaption(
            Guid teamId,
            Guid photoId,
            [FromBody] string caption,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateTeamPhotoCaption Team={TeamId}, Photo={PhotoId}", teamId, photoId);

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

            var photo = await teamPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.TeamId != teamId)
            {
                return NotFound();
            }

            photo.Caption = caption ?? string.Empty;
            photo.LastUpdatedByUserId = UserId;
            photo.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedPhoto = await teamPhotoManager.UpdateAsync(photo, UserId, cancellationToken);
            return Ok(updatedPhoto.ToV2Dto());
        }

        /// <summary>
        /// Deletes a team photo. Only team leads can delete photos.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Photo deleted.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or photo not found.</response>
        [HttpDelete("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePhoto(
            Guid teamId,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteTeamPhoto Team={TeamId}, Photo={PhotoId}", teamId, photoId);

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

            var photo = await teamPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.TeamId != teamId)
            {
                return NotFound();
            }

            await teamPhotoManager.HardDeleteAsync(photoId, cancellationToken);
            return NoContent();
        }
    }
}
