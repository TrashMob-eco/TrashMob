namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing event photos.
    /// </summary>
    [Route("api/events/{eventId}/photos")]
    public class EventPhotosController(
        IEventManager eventManager,
        IEventAttendeeManager eventAttendeeManager,
        IEventPhotoManager eventPhotoManager,
        IImageManager imageManager)
        : SecureController
    {
        /// <summary>
        /// Gets all photos for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoType">Optional filter by photo type (Before, During, After).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventPhoto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventPhotos(
            Guid eventId,
            [FromQuery] EventPhotoType? photoType,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound("Event not found");
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

            return Ok(photos);
        }

        /// <summary>
        /// Gets a specific photo.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{photoId}")]
        [ProducesResponseType(typeof(EventPhoto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPhoto(Guid eventId, Guid photoId, CancellationToken cancellationToken)
        {
            var photo = await eventPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.EventId != eventId)
            {
                return NotFound("Photo not found");
            }

            return Ok(photo);
        }

        /// <summary>
        /// Uploads a photo for an event. Only event leads and attendees can upload photos.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="photoType">The type of photo (Before, During, After).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPhoto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadPhoto(
            Guid eventId,
            [FromForm] ImageUpload imageUpload,
            [FromQuery] EventPhotoType photoType = EventPhotoType.During,
            CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound("Event not found");
            }

            // Check if user is event lead or attendee
            var isLead = mobEvent.CreatedByUserId == UserId;
            var attendees = await eventAttendeeManager.GetAsync(ea => ea.EventId == eventId && ea.UserId == UserId, cancellationToken);
            var isAttendee = attendees.Any();

            if (!isLead && !isAttendee)
            {
                return Forbid();
            }

            // Create the photo record
            var photoId = Guid.NewGuid();
            var eventPhoto = new EventPhoto
            {
                Id = photoId,
                EventId = eventId,
                PhotoType = photoType,
                Caption = string.Empty,
                UploadedByUserId = UserId,
                UploadedDate = DateTimeOffset.UtcNow,
                ModerationStatus = PhotoModerationStatus.Pending
            };

            // Upload to blob storage
            imageUpload.ParentId = photoId;
            imageUpload.ImageType = ImageTypeEnum.EventPhoto;
            await imageManager.UploadImageAsync(imageUpload);

            // Get the image URLs and save photo record
            var imageUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.EventPhoto, ImageSizeEnum.Reduced, cancellationToken);
            var thumbnailUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.EventPhoto, ImageSizeEnum.Thumb, cancellationToken);

            eventPhoto.ImageUrl = imageUrl;
            eventPhoto.ThumbnailUrl = thumbnailUrl;

            var createdPhoto = await eventPhotoManager.AddAsync(eventPhoto, UserId, cancellationToken);

            TrackEvent(nameof(UploadPhoto));

            return CreatedAtAction(nameof(GetPhoto), new { eventId, photoId = createdPhoto.Id }, createdPhoto);
        }

        /// <summary>
        /// Updates a photo's metadata. Only the uploader or event lead can update.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventPhoto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePhoto(
            Guid eventId,
            Guid photoId,
            [FromBody] UpdateEventPhotoRequest request,
            CancellationToken cancellationToken)
        {
            var photo = await eventPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.EventId != eventId)
            {
                return NotFound("Photo not found");
            }

            // Check if user is uploader or event lead
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            var isLead = mobEvent?.CreatedByUserId == UserId;
            var isUploader = photo.UploadedByUserId == UserId;

            if (!isLead && !isUploader)
            {
                return Forbid();
            }

            var updatedPhoto = await eventPhotoManager.UpdatePhotoMetadataAsync(
                photoId,
                request.Caption,
                request.PhotoType,
                UserId,
                cancellationToken);

            return Ok(updatedPhoto);
        }

        /// <summary>
        /// Deletes a photo. Only the uploader or event lead can delete.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePhoto(
            Guid eventId,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            var photo = await eventPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.EventId != eventId)
            {
                return NotFound("Photo not found");
            }

            // Check if user is uploader or event lead
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            var isLead = mobEvent?.CreatedByUserId == UserId;
            var isUploader = photo.UploadedByUserId == UserId;

            if (!isLead && !isUploader)
            {
                return Forbid();
            }

            await eventPhotoManager.DeletePhotoAsync(photoId, UserId, cancellationToken);

            TrackEvent(nameof(DeletePhoto));

            return NoContent();
        }

        /// <summary>
        /// Flags a photo for moderation review.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="request">The flag request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{photoId}/flag")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FlagPhoto(
            Guid eventId,
            Guid photoId,
            [FromBody] EventPhotoFlagRequest request,
            CancellationToken cancellationToken)
        {
            var photo = await eventPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.EventId != eventId)
            {
                return NotFound("Photo not found");
            }

            var result = await eventPhotoManager.FlagPhotoAsync(photoId, UserId, request.Reason, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to flag photo");
            }

            TrackEvent(nameof(FlagPhoto));

            return Ok(new { message = "Photo flagged for review" });
        }
    }

    /// <summary>
    /// Request model for updating an event photo.
    /// </summary>
    public class UpdateEventPhotoRequest
    {
        /// <summary>
        /// Gets or sets the photo caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the photo type.
        /// </summary>
        public EventPhotoType PhotoType { get; set; }
    }

    /// <summary>
    /// Request model for flagging an event photo.
    /// </summary>
    public class EventPhotoFlagRequest
    {
        /// <summary>
        /// Gets or sets the reason for flagging.
        /// </summary>
        public string Reason { get; set; }
    }
}
