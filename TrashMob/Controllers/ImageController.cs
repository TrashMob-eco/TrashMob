namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Poco;

    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public ImageController() 
        {
        }

        [HttpPost]
        public Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload)
        {
            return null;
        }
    }
}
