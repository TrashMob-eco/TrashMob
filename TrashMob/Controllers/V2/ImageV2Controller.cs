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
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for event image upload and deletion.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/image")]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    public class ImageV2Controller(
        IImageManager imageManager,
        IEventManager eventManager,
        IAuthorizationService authorizationService,
        ILogger<ImageV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Uploads an image for an event. Requires event lead authorization.
        /// </summary>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK on success.</returns>
        /// <response code="200">Image uploaded successfully.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(imageUpload.ParentId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            logger.LogInformation("V2 UploadImage for event {EventId}", imageUpload.ParentId);

            await imageManager.UploadImageAsync(imageUpload);

            return Ok();
        }

        /// <summary>
        /// Deletes an image for an event. Requires event lead authorization.
        /// </summary>
        /// <param name="parentId">The parent entity ID.</param>
        /// <param name="imageType">The image type.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Image deleted successfully.</response>
        /// <response code="400">Image could not be deleted.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpDelete]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteImage(Guid parentId, ImageTypeEnum imageType, CancellationToken cancellationToken = default)
        {
            var mobEvent = await eventManager.GetAsync(parentId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            logger.LogInformation("V2 DeleteImage for event {EventId}", parentId);

            var deleted = await imageManager.DeleteImageAsync(parentId, imageType);

            if (deleted)
            {
                return NoContent();
            }

            return Problem("The image could not be deleted.", statusCode: StatusCodes.Status400BadRequest);
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policyName)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var result = await authorizationService.AuthorizeAsync(User, resource, policyName);
            return result.Succeeded;
        }
    }
}
