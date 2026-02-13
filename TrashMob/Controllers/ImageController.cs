namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing images, including upload and deletion.
    /// </summary>
    [Route("api/image")]
    [ApiController]
    public class ImageController : SecureController
    {
        private readonly IEventManager eventManager;
        private readonly IImageManager imageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageController"/> class.
        /// </summary>
        /// <param name="imageManager">The image manager.</param>
        /// <param name="eventManager">The event manager.</param>
        public ImageController(IImageManager imageManager, IEventManager eventManager)
        {
            this.imageManager = imageManager;
            this.eventManager = eventManager;
        }

        /// <summary>
        /// Uploads an image for an event.
        /// </summary>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(imageUpload.ParentId, cancellationToken).ConfigureAwait(false);
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserIsEventLead);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await imageManager.UploadImage(imageUpload);

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
        public async Task<IActionResult> DeleteImage(Guid parentId, ImageTypeEnum imageType,
            CancellationToken cancellationToken)
        {
            var deleted = await imageManager.DeleteImage(parentId, imageType);
            if (deleted)
            {
                return Ok();
            }

            return BadRequest("The image is not deleted");
        }
    }
}