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

    [Route("api/image")]
    [ApiController]
    public class ImageController : SecureController
    {
        private readonly IEventManager eventManager;
        private readonly IImageManager imageManager;

        public ImageController(IImageManager imageManager, IEventManager eventManager)
        {
            this.imageManager = imageManager;
            this.eventManager = eventManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var mobEvent = eventManager.GetAsync(imageUpload.ParentId, cancellationToken);
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await imageManager.UploadImage(imageUpload);

            return Ok();
        }

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