namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Controller for retrieving minimum and recommended app version requirements.
    /// </summary>
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/appversion")]
    public class AppVersionController(IConfiguration configuration) : BaseController
    {
        /// <summary>
        /// Gets the minimum and recommended app version requirements.
        /// </summary>
        /// <remarks>
        /// Returns version strings used by the mobile app to determine
        /// whether a forced update or soft nudge is needed.
        /// No authentication required.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(AppVersionInfo), StatusCodes.Status200OK)]
        public IActionResult GetAppVersion()
        {
            var appVersionInfo = new AppVersionInfo
            {
                MinimumVersion = configuration["AppVersion:MinimumVersion"] ?? "1.0.0",
                RecommendedVersion = configuration["AppVersion:RecommendedVersion"] ?? "1.0.0",
            };

            return Ok(appVersionInfo);
        }
    }
}
