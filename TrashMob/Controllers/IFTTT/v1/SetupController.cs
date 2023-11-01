namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Mvc;
    
    [Route("api/ifttt/v1/test/[controller]")]
    [ApiController]

    public class SetupController : Controller
    {
        [HttpPost]
        public ActionResult GetInfo()
        {
            var dataResponse = new DataResponse();

            var accessToken = HttpContext.Request.Headers.Authorization;

            var sampleResponse = new SampleResponse
            {
                accessToken = accessToken,
            };

            dataResponse.Data = sampleResponse;

            return Ok(dataResponse);
        }
    }
}
