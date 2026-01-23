namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;

    /// <summary>
    /// Controller for IFTTT status, providing endpoints for status checks.
    /// </summary>
    [Route("api/ifttt/v1/[controller]")]
    [ApiController]
    public class StatusController : Controller
    {
        /// <summary>
        /// Gets status information for IFTTT integration.
        /// </summary>
        /// <remarks>Action result with status response.</remarks>
        [HttpGet]
        [TypeFilter(typeof(ChannelKeyAuthenticationFilter))]
        public ActionResult GetInfo()
        {
            var dataResponse = new DataResponse();

            return Ok(dataResponse);
        }
    }
}