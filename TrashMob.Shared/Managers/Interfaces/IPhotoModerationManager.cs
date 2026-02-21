namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Manager interface for photo moderation operations.
    /// Handles moderation for both LitterImage and TeamPhoto entities.
    /// </summary>
    public interface IPhotoModerationManager
    {
        /// <summary>
        /// Gets photos pending moderation review.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of photos pending review.</returns>
        Task<PaginatedList<PhotoModerationItem>> GetPendingPhotosAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets photos flagged by users for review.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of flagged photos.</returns>
        Task<PaginatedList<PhotoModerationItem>> GetFlaggedPhotosAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets recently moderated photos.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of recently moderated photos.</returns>
        Task<PaginatedList<PhotoModerationItem>> GetRecentlyModeratedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves a photo for display.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="photoId">Photo identifier.</param>
        /// <param name="adminUserId">Admin user performing the action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        Task<PhotoModerationItem> ApprovePhotoAsync(string photoType, Guid photoId, Guid adminUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Rejects a photo and removes it from display.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="photoId">Photo identifier.</param>
        /// <param name="reason">Reason for rejection.</param>
        /// <param name="adminUserId">Admin user performing the action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        Task<PhotoModerationItem> RejectPhotoAsync(string photoType, Guid photoId, string reason, Guid adminUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dismisses a flag on a photo (false positive).
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="photoId">Photo identifier.</param>
        /// <param name="adminUserId">Admin user performing the action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        Task<PhotoModerationItem> DismissFlagAsync(string photoType, Guid photoId, Guid adminUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Flags a photo for review (user-initiated).
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="photoId">Photo identifier.</param>
        /// <param name="reason">Reason for flagging.</param>
        /// <param name="userId">User flagging the photo.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created photo flag.</returns>
        Task<PhotoFlag> FlagPhotoAsync(string photoType, Guid photoId, string reason, Guid userId, CancellationToken cancellationToken = default);
    }
}
