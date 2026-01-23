namespace TrashMob.Controllers.IFTTT
{
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for IFTTT setup, providing endpoints for test setup and authentication.
    /// </summary>
    [Route("api/ifttt/v1/test/[controller]")]
    [ApiController]
    public class SetupController : Controller
    {
        private readonly IKeyVaultManager keyVaultManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupController"/> class.
        /// </summary>
        /// <param name="keyVaultManager">The key vault manager.</param>
        public SetupController(IKeyVaultManager keyVaultManager)
        {
            this.keyVaultManager = keyVaultManager;
        }

        /// <summary>
        /// Gets test setup information for IFTTT authentication.
        /// </summary>
        /// <remarks>Action result with sample response and access token.</remarks>
        [HttpPost]
        [TypeFilter(typeof(ChannelKeyAuthenticationFilter))]
        public ActionResult GetInfo()
        {
            var dataResponse = new DataResponse();

            var accessToken = keyVaultManager.GetSecret("IFTTTAuthToken");

            var sampleResponse = new SampleResponse
            {
                accessToken = accessToken,
                samples = new Sample(),
            };

            dataResponse.Data = sampleResponse;

            return Ok(dataResponse);
        }
    }
}