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
    /// V2 controller for user-initiated photo flagging.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/photos")]
    public class PhotoFlagV2Controller(
        IPhotoModerationManager photoModerationManager,
        ILogger<PhotoFlagV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Flags a photo for review by moderators.
        /// </summary>
        /// <param name="photoType">Type of photo ("LitterImage" or "TeamPhoto").</param>
        /// <param name="id">Photo identifier.</param>
        /// <param name="request">The flag request containing the reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created photo flag.</returns>
        /// <response code="200">Photo flagged successfully.</response>
        /// <response code="400">Reason is required.</response>
        /// <response code="404">Photo not found.</response>
        [HttpPost("{photoType}/{id}/flag")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PhotoFlagDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FlagPhoto(string photoType, Guid id, [FromBody] FlagPhotoRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return Problem("Reason is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 FlagPhoto: photoType={PhotoType}, id={Id}", photoType, id);

            var result = await photoModerationManager.FlagPhotoAsync(photoType, id, request.Reason, UserId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToV2Dto());
        }
    }
}
