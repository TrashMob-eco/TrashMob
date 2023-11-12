namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/ifttt/v1/test/[controller]")]
    [ApiController]

    public class SetupController : Controller
    {
        private readonly IKeyVaultManager keyVaultManager;

        public SetupController(IKeyVaultManager keyVaultManager)
        {
            this.keyVaultManager = keyVaultManager;
        }

        [HttpPost]
        [TypeFilter(typeof(ChannelKeyAuthenticationFilter))]
        public ActionResult GetInfo()
        {
            var dataResponse = new DataResponse();

            var accessToken = keyVaultManager.GetSecret("IFTTTAuthToken");

            var sampleResponse = new SampleResponse
            {
                accessToken = accessToken,
                samples = new Sample()
            };

            dataResponse.Data = sampleResponse;

            return Ok(dataResponse);
        }
    }
}
