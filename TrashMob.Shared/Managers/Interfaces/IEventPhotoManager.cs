namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing event photos.
    /// </summary>
    public interface IEventPhotoManager : IKeyedManager<EventPhoto>
    {
        /// <summary>
        /// Gets all photos for an event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="includeModerated">Whether to include moderated/hidden photos.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The collection of event photos.</returns>
        Task<IEnumerable<EventPhoto>> GetByEventIdAsync(
            Guid eventId,
            bool includeModerated = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets photos for an event filtered by photo type.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="photoType">The type of photos to retrieve (Before, During, After).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The collection of event photos matching the criteria.</returns>
        Task<IEnumerable<EventPhoto>> GetByEventIdAndTypeAsync(
            Guid eventId,
            EventPhotoType photoType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new event photo with image data.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="photoType">The type of photo (Before, During, After).</param>
        /// <param name="caption">Optional caption for the photo.</param>
        /// <param name="imageData">The base64-encoded image data.</param>
        /// <param name="userId">The user uploading the photo.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The created event photo.</returns>
        Task<EventPhoto> AddPhotoAsync(
            Guid eventId,
            EventPhotoType photoType,
            string caption,
            string imageData,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an event photo's metadata (caption, type).
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="caption">The new caption.</param>
        /// <param name="photoType">The new photo type.</param>
        /// <param name="userId">The user making the update.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated event photo.</returns>
        Task<EventPhoto> UpdatePhotoMetadataAsync(
            Guid photoId,
            string caption,
            EventPhotoType photoType,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes a photo (marks as rejected).
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="userId">The user performing the deletion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if deleted, false otherwise.</returns>
        Task<bool> DeletePhotoAsync(
            Guid photoId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Flags a photo for review.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="userId">The user flagging the photo.</param>
        /// <param name="reason">The reason for flagging.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if flagged successfully, false otherwise.</returns>
        Task<bool> FlagPhotoAsync(
            Guid photoId,
            Guid userId,
            string reason,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets photos pending moderation review.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The collection of photos awaiting moderation.</returns>
        Task<IEnumerable<EventPhoto>> GetPendingModerationAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves a flagged photo.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="moderatorId">The admin user approving the photo.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated event photo.</returns>
        Task<EventPhoto> ApprovePhotoAsync(
            Guid photoId,
            Guid moderatorId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rejects a flagged photo.
        /// </summary>
        /// <param name="photoId">The photo identifier.</param>
        /// <param name="moderatorId">The admin user rejecting the photo.</param>
        /// <param name="reason">The reason for rejection.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated event photo.</returns>
        Task<EventPhoto> RejectPhotoAsync(
            Guid photoId,
            Guid moderatorId,
            string reason,
            CancellationToken cancellationToken = default);
    }
}
