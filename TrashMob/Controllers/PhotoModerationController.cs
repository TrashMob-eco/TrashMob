namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for photo moderation operations.
    /// Includes admin endpoints for moderation and user endpoints for flagging.
    /// </summary>
    [Route("api/admin/photos")]
    public class PhotoModerationController(IPhotoModerationManager photoModerationManager)
        : SecureController
    {

        /// <summary>
        /// Gets photos pending moderation review. Admin only.
        /// </summary>
        /// <param name="page">Page number (1-based, default 1).</param>
        /// <param name="pageSize">Items per page (default 50).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of pending photos.</returns>
        [HttpGet("pending")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<PhotoModerationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingPhotos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await photoModerationManager.GetPendingPhotosAsync(page, pageSize, cancellationToken);
            TrackEvent(nameof(GetPendingPhotos));

            return Ok(result);
        }

        /// <summary>
        /// Gets photos flagged by users for review. Admin only.
        /// </summary>
        /// <param name="page">Page number (1-based, default 1).</param>
        /// <param name="pageSize">Items per page (default 50).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of flagged photos.</returns>
        [HttpGet("flagged")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<PhotoModerationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFlaggedPhotos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await photoModerationManager.GetFlaggedPhotosAsync(page, pageSize, cancellationToken);
            TrackEvent(nameof(GetFlaggedPhotos));

            return Ok(result);
        }

        /// <summary>
        /// Gets recently moderated photos. Admin only.
        /// </summary>
        /// <param name="page">Page number (1-based, default 1).</param>
        /// <param name="pageSize">Items per page (default 50).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of recently moderated photos.</returns>
        [HttpGet("moderated")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<PhotoModerationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecentlyModerated(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await photoModerationManager.GetRecentlyModeratedAsync(page, pageSize, cancellationToken);
            TrackEvent(nameof(GetRecentlyModerated));

            return Ok(result);
        }

        /// <summary>
        /// Approves a photo for display. Admin only.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        [HttpPost("{photoType}/{id}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoModerationItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApprovePhoto(
            string photoType,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await photoModerationManager.ApprovePhotoAsync(photoType, id, UserId, cancellationToken);
                TrackEvent(nameof(ApprovePhoto));

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Rejects a photo and removes it from display. Admin only.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="request">Rejection request with reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        [HttpPost("{photoType}/{id}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoModerationItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectPhoto(
            string photoType,
            Guid id,
            [FromBody] RejectPhotoRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request?.Reason))
            {
                return BadRequest("Rejection reason is required.");
            }

            try
            {
                var result = await photoModerationManager.RejectPhotoAsync(photoType, id, request.Reason, UserId, cancellationToken);
                TrackEvent(nameof(RejectPhoto));

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Dismisses a flag on a photo (false positive). Admin only.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        [HttpPost("{photoType}/{id}/dismiss")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoModerationItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DismissFlag(
            string photoType,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await photoModerationManager.DismissFlagAsync(photoType, id, UserId, cancellationToken);
                TrackEvent(nameof(DismissFlag));

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    /// <summary>
    /// Controller for user photo flagging.
    /// </summary>
    [Route("api/photos")]
    public class PhotoFlagController(IPhotoModerationManager photoModerationManager)
        : SecureController
    {

        /// <summary>
        /// Flags a photo for review. Any authenticated user.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="request">Flag request with reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created photo flag.</returns>
        [HttpPost("{photoType}/{id}/flag")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoFlag), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FlagPhoto(
            string photoType,
            Guid id,
            [FromBody] FlagPhotoRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request?.Reason))
            {
                return BadRequest("Flag reason is required.");
            }

            try
            {
                var result = await photoModerationManager.FlagPhotoAsync(photoType, id, request.Reason, UserId, cancellationToken);
                TrackEvent(nameof(FlagPhoto));

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    /// <summary>
    /// Request model for rejecting a photo.
    /// </summary>
    public class RejectPhotoRequest
    {
        /// <summary>
        /// Gets or sets the reason for rejection.
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for flagging a photo.
    /// </summary>
    public class FlagPhotoRequest
    {
        /// <summary>
        /// Gets or sets the reason for flagging.
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
}
