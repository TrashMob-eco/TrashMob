namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageManager imageManager;

        public ImageController(IImageManager imageManager) 
        {
            this.imageManager = imageManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload)
        {
            await imageManager.UploadImage(imageUpload);

            return Ok();
        }
    }
}
