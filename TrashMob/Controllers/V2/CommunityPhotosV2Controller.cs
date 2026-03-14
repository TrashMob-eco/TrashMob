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
    /// V2 controller for community photo gallery operations as a nested resource under communities.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{slug}/photos")]
    public class CommunityPhotosV2Controller(
        ICommunityManager communityManager,
        IPartnerPhotoManager partnerPhotoManager,
        IImageManager imageManager,
        IAuthorizationService authorizationService,
        ILogger<CommunityPhotosV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all photos for a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of community photos.</returns>
        /// <response code="200">Returns the community photos.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PartnerPhotoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityPhotos(string slug, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCommunityPhotos requested Slug={Slug}", slug);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            var photos = await partnerPhotoManager.GetByPartnerIdAsync(community.Id, cancellationToken);
            return Ok(photos.Select(p => p.ToV2Dto()));
        }

        /// <summary>
        /// Uploads a photo for a community. Only community admins can upload photos.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="imageUpload">The image upload data (multipart/form-data).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created photo.</returns>
        /// <response code="201">Photo uploaded successfully.</response>
        /// <response code="400">Invalid upload data.</response>
        /// <response code="403">User is not a community admin.</response>
        /// <response code="404">Community not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerPhotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadCommunityPhoto(
            string slug,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadCommunityPhoto requested Slug={Slug}", slug);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            // Create the partner photo record
            var photoId = Guid.NewGuid();
            var partnerPhoto = new PartnerPhoto
            {
                Id = photoId,
                PartnerId = community.Id,
                Caption = string.Empty,
            };

            // Upload to blob storage
            imageUpload.ParentId = photoId;
            imageUpload.ImageType = ImageTypeEnum.PartnerPhoto;
            await imageManager.UploadImageAsync(imageUpload);

            // Get the image URL and save photo record
            var imageUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.PartnerPhoto, ImageSizeEnum.Reduced, cancellationToken);
            partnerPhoto.ImageUrl = imageUrl;
            var createdPhoto = await partnerPhotoManager.AddAsync(partnerPhoto, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetCommunityPhotos), new { slug }, createdPhoto.ToV2Dto());
        }

        /// <summary>
        /// Updates a community photo caption. Only community admins can update photos.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="caption">The new caption text.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo.</returns>
        /// <response code="200">Returns the updated photo.</response>
        /// <response code="403">User is not a community admin.</response>
        /// <response code="404">Community or photo not found.</response>
        [HttpPut("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerPhotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCommunityPhotoCaption(
            string slug,
            Guid photoId,
            [FromBody] string caption,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateCommunityPhotoCaption requested Slug={Slug}, PhotoId={PhotoId}", slug, photoId);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var photo = await partnerPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.PartnerId != community.Id)
            {
                return NotFound();
            }

            photo.Caption = caption ?? string.Empty;
            photo.LastUpdatedByUserId = UserId;
            photo.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedPhoto = await partnerPhotoManager.UpdateAsync(photo, UserId, cancellationToken);
            return Ok(updatedPhoto.ToV2Dto());
        }

        /// <summary>
        /// Deletes a community photo. Only community admins can delete photos.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Photo deleted.</response>
        /// <response code="403">User is not a community admin.</response>
        /// <response code="404">Community or photo not found.</response>
        [HttpDelete("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCommunityPhoto(
            string slug,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteCommunityPhoto requested Slug={Slug}, PhotoId={PhotoId}", slug, photoId);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var photo = await partnerPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.PartnerId != community.Id)
            {
                return NotFound();
            }

            // HardDelete removes from blob storage and database
            await partnerPhotoManager.HardDeleteAsync(photoId, cancellationToken);
            return NoContent();
        }

        // ============================================================================
        // Private Helpers
        // ============================================================================

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
