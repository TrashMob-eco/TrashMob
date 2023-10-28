namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    
    [Route("api/ifttt/v1/test/[controller]")]
    [ApiController]

    public class SetupController : SecureController
    {
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [Authorize(Policy = AuthorizationPolicyConstants.IftttServiceKey)]
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
