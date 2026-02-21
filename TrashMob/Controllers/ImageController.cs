namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing images, including upload and deletion.
    /// </summary>
    [Route("api/image")]
    public class ImageController(
        IImageManager imageManager,
        IEventManager eventManager)
        : SecureController
    {

        /// <summary>
        /// Uploads an image for an event.
        /// </summary>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UploadImageAsync([FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(imageUpload.ParentId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await imageManager.UploadImageAsync(imageUpload);

            return Ok();
        }

        /// <summary>
        /// Deletes an image for an event.
        /// </summary>
        /// <param name="parentId">The parent entity ID.</param>
        /// <param name="imageType">The image type.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpDelete]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteImageAsync(Guid parentId, ImageTypeEnum imageType,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(parentId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var deleted = await imageManager.DeleteImageAsync(parentId, imageType);
            if (deleted)
            {
                return NoContent();
            }

            return BadRequest("The image is not deleted");
        }
    }
}
