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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for admin photo moderation operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin/photos")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class PhotoModerationV2Controller(
        IPhotoModerationManager photoModerationManager,
        ILogger<PhotoModerationV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets photos pending moderation review.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of photos pending review.</returns>
        /// <response code="200">Returns pending photos.</response>
        [HttpGet("pending")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<PhotoModerationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingPhotos([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetPendingPhotos: page={Page}, pageSize={PageSize}", page, pageSize);

            var result = await photoModerationManager.GetPendingPhotosAsync(page, pageSize, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets photos flagged by users for review.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of flagged photos.</returns>
        /// <response code="200">Returns flagged photos.</response>
        [HttpGet("flagged")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<PhotoModerationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFlaggedPhotos([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetFlaggedPhotos: page={Page}, pageSize={PageSize}", page, pageSize);

            var result = await photoModerationManager.GetFlaggedPhotosAsync(page, pageSize, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets recently moderated photos.
        /// </summary>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of recently moderated photos.</returns>
        /// <response code="200">Returns recently moderated photos.</response>
        [HttpGet("moderated")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<PhotoModerationItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecentlyModerated([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetRecentlyModerated: page={Page}, pageSize={PageSize}", page, pageSize);

            var result = await photoModerationManager.GetRecentlyModeratedAsync(page, pageSize, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Approves a photo for display.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        /// <response code="200">Photo approved successfully.</response>
        /// <response code="404">Photo not found.</response>
        [HttpPost("{photoType}/{id}/approve")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoModerationItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApprovePhoto(string photoType, Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 ApprovePhoto: photoType={PhotoType}, id={Id}", photoType, id);

            var result = await photoModerationManager.ApprovePhotoAsync(photoType, id, UserId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Rejects a photo and removes it from display.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="request">The rejection request containing the reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        /// <response code="200">Photo rejected successfully.</response>
        /// <response code="400">Reason is required.</response>
        /// <response code="404">Photo not found.</response>
        [HttpPost("{photoType}/{id}/reject")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoModerationItem), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectPhoto(string photoType, Guid id, [FromBody] RejectPhotoRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return Problem("Reason is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 RejectPhoto: photoType={PhotoType}, id={Id}", photoType, id);

            var result = await photoModerationManager.RejectPhotoAsync(photoType, id, request.Reason, UserId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Dismisses a flag on a photo (false positive).
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated photo moderation item.</returns>
        /// <response code="200">Flag dismissed successfully.</response>
        /// <response code="404">Photo not found.</response>
        [HttpPost("{photoType}/{id}/dismiss")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoModerationItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DismissFlag(string photoType, Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DismissFlag: photoType={PhotoType}, id={Id}", photoType, id);

            var result = await photoModerationManager.DismissFlagAsync(photoType, id, UserId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
