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
    /// V2 controller for managing event photos.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/photos")]
    public class EventPhotosV2Controller(
        IEventPhotoManager eventPhotoManager,
        IEventManager eventManager,
        IEventAttendeeManager eventAttendeeManager,
        IImageManager imageManager,
        ILogger<EventPhotosV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all photos for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoType">Optional filter by photo type (Before, During, After).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the photo list.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<EventPhotoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventPhotos(
            Guid eventId,
            [FromQuery] EventPhotoType? photoType,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventPhotos for Event={EventId}, PhotoType={PhotoType}", eventId, photoType);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            IEnumerable<EventPhoto> photos;
            if (photoType.HasValue)
            {
                photos = await eventPhotoManager.GetByEventIdAndTypeAsync(eventId, photoType.Value, cancellationToken);
            }
            else
            {
                photos = await eventPhotoManager.GetByEventIdAsync(eventId, false, cancellationToken);
            }

            var dtos = photos.Select(p => p.ToV2Dto()).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Uploads a photo for an event. Only event leads and attendees can upload.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="photoType">The type of photo (Before, During, After).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Photo created.</response>
        /// <response code="403">Not event lead or attendee.</response>
        /// <response code="404">Event not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPhotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadPhoto(
            Guid eventId,
            [FromForm] ImageUpload imageUpload,
            [FromQuery] EventPhotoType photoType = EventPhotoType.During,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UploadPhoto for Event={EventId}, Type={PhotoType}", eventId, photoType);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            var isLead = mobEvent.CreatedByUserId == UserId;
            var attendees = await eventAttendeeManager.GetAsync(ea => ea.EventId == eventId && ea.UserId == UserId, cancellationToken);
            var isAttendee = attendees.Any();

            if (!isLead && !isAttendee)
            {
                return Forbid();
            }

            var photoId = Guid.NewGuid();
            var eventPhoto = new EventPhoto
            {
                Id = photoId,
                EventId = eventId,
                PhotoType = photoType,
                Caption = string.Empty,
                UploadedByUserId = UserId,
                UploadedDate = DateTimeOffset.UtcNow,
                ModerationStatus = PhotoModerationStatus.Pending,
            };

            imageUpload.ParentId = photoId;
            imageUpload.ImageType = ImageTypeEnum.EventPhoto;
            await imageManager.UploadImageAsync(imageUpload);

            var imageUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.EventPhoto, ImageSizeEnum.Reduced, cancellationToken);
            var thumbnailUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.EventPhoto, ImageSizeEnum.Thumb, cancellationToken);

            eventPhoto.ImageUrl = imageUrl;
            eventPhoto.ThumbnailUrl = thumbnailUrl;

            var createdPhoto = await eventPhotoManager.AddAsync(eventPhoto, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetEventPhotos), new { eventId }, createdPhoto.ToV2Dto());
        }

        /// <summary>
        /// Deletes a photo. Only the uploader or event lead can delete.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Photo deleted.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Photo not found.</response>
        [HttpDelete("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePhoto(
            Guid eventId,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePhoto Photo={PhotoId} from Event={EventId}", photoId, eventId);

            var photo = await eventPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.EventId != eventId)
            {
                return NotFound();
            }

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            var isLead = mobEvent?.CreatedByUserId == UserId;
            var isUploader = photo.UploadedByUserId == UserId;

            if (!isLead && !isUploader)
            {
                return Forbid();
            }

            await eventPhotoManager.DeletePhotoAsync(photoId, UserId, cancellationToken);

            return NoContent();
        }
    }
}
