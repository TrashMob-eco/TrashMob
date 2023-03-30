namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/image")]
    [ApiController]
    public class ImageController : SecureController
    {
        private readonly IImageManager imageManager;
        private readonly IEventManager eventManager;

        public ImageController(IImageManager imageManager, IEventManager eventManager) 
        {
            this.imageManager = imageManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload, CancellationToken cancellationToken)
        {
            var mobEvent = eventManager.GetAsync(imageUpload.ParentId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await imageManager.UploadImage(imageUpload);

            return Ok();
        }
    }
}
