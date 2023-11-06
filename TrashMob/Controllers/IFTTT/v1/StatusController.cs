namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;

    [Route("api/ifttt/v1/[controller]")]
    [ApiController]
    public class StatusController : Controller
    {
        [HttpGet]
        [TypeFilter(typeof(ChannelKeyAuthenticationFilter))]
        public ActionResult GetInfo()
        {
            var dataResponse = new DataResponse();

            return Ok(dataResponse);
        }
    }
}
