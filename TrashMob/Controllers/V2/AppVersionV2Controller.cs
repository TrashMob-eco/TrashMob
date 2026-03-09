namespace TrashMob.Controllers.V2
{
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco;

    /// <summary>
    /// V2 controller for app version requirements.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/appversion")]
    public class AppVersionV2Controller(
        IConfiguration configuration,
        ILogger<AppVersionV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets the minimum and recommended app version requirements.
        /// </summary>
        /// <returns>The app version info.</returns>
        /// <response code="200">Returns the version requirements.</response>
        [HttpGet]
        [ProducesResponseType(typeof(AppVersionInfo), StatusCodes.Status200OK)]
        public IActionResult GetAppVersion()
        {
            logger.LogInformation("V2 GetAppVersion requested");

            var appVersionInfo = new AppVersionInfo
            {
                MinimumVersion = configuration["AppVersion:MinimumVersion"] ?? "1.0.0",
                RecommendedVersion = configuration["AppVersion:RecommendedVersion"] ?? "1.0.0",
            };

            return Ok(appVersionInfo);
        }
    }
}
