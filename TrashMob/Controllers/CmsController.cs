namespace TrashMob.Controllers
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;

    /// <summary>
    /// Controller for proxying requests to the Strapi CMS.
    /// </summary>
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/cms")]
    public class CmsController : SecureController
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string strapiBaseUrl;

        /// <summary>
        /// Initializes a new instance of the CmsController.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory for creating Strapi clients.</param>
        /// <param name="configuration">Application configuration.</param>
        public CmsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            strapiBaseUrl = configuration["StrapiBaseUrl"] ?? string.Empty;
        }

        /// <summary>
        /// Gets hero section content from CMS. Public endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the hero section content for the home page.</remarks>
        [HttpGet("hero-section")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetHeroSection(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var response = await client.GetAsync($"{strapiBaseUrl}/api/hero-section?populate=*", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets "What is TrashMob" section content from CMS. Public endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the "What is TrashMob" section content for the home page.</remarks>
        [HttpGet("what-is-trashmob")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetWhatIsTrashmob(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var response = await client.GetAsync($"{strapiBaseUrl}/api/what-is-trashmob?populate=*", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets "Getting Started" section content from CMS. Public endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the "Getting Started" section content for the home page.</remarks>
        [HttpGet("getting-started")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetGettingStarted(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var response = await client.GetAsync($"{strapiBaseUrl}/api/getting-started?populate=*", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets the Strapi admin URL. Admin only.
        /// </summary>
        /// <remarks>Returns the URL to the Strapi admin panel for content management.</remarks>
        [HttpGet("admin-url")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public IActionResult GetAdminUrl()
        {
            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            return Ok(new { adminUrl = $"{strapiBaseUrl}/admin" });
        }
    }
}
