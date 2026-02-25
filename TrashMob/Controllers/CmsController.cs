namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;

    /// <summary>
    /// Controller for proxying requests to the Strapi CMS.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the CmsController.
    /// </remarks>
    /// <param name="httpClientFactory">HTTP client factory for creating Strapi clients.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="logger">Logger for telemetry.</param>
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/cms")]
    public class CmsController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CmsController> logger) : BaseController(logger)
    {
        private readonly string strapiBaseUrl = configuration["StrapiBaseUrl"] ?? string.Empty;

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
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
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
            TrackEvent(nameof(GetHeroSection));
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
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
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
            TrackEvent(nameof(GetWhatIsTrashmob));
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
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
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
            TrackEvent(nameof(GetGettingStarted));
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets published news posts from CMS with pagination. Public endpoint.
        /// </summary>
        /// <param name="page">Page number (1-based, default 1).</param>
        /// <param name="pageSize">Items per page (default 10, max 100).</param>
        /// <param name="category">Optional category slug filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns paginated news posts sorted by publish date descending.</remarks>
        [HttpGet("news-posts")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string category = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            pageSize = Math.Clamp(pageSize, 1, 100);

            var url = $"{strapiBaseUrl}/api/news-posts?populate=*&sort=publishedAt:desc&pagination[page]={page}&pagination[pageSize]={pageSize}";

            if (!string.IsNullOrWhiteSpace(category))
            {
                url += $"&filters[category][slug][$eq]={Uri.EscapeDataString(category)}";
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var response = await client.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            TrackEvent(nameof(GetNewsPosts));
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets a single news post by slug from CMS. Public endpoint.
        /// </summary>
        /// <param name="slug">URL-friendly post identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the news post matching the given slug, or empty data array if not found.</remarks>
        [HttpGet("news-posts/{slug}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsPostBySlug(string slug, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var response = await client.GetAsync(
                $"{strapiBaseUrl}/api/news-posts?filters[slug][$eq]={Uri.EscapeDataString(slug)}&populate=*",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            TrackEvent(nameof(GetNewsPostBySlug));
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets all news categories from CMS. Public endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns all news categories sorted alphabetically.</remarks>
        [HttpGet("news-categories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsCategories(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var response = await client.GetAsync($"{strapiBaseUrl}/api/news-categories?sort=name:asc", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            TrackEvent(nameof(GetNewsCategories));
            return Content(content, "application/json");
        }

        /// <summary>
        /// Gets an RSS 2.0 feed of recent news posts. Public endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the 20 most recent published news posts as an RSS 2.0 XML feed.</remarks>
        [HttpGet("news-feed")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsFeed(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            var client = httpClientFactory.CreateClient("Strapi");
            var url = $"{strapiBaseUrl}/api/news-posts?populate=*&sort=publishedAt:desc&pagination[page]=1&pagination[pageSize]=20";
            var response = await client.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var doc = JsonDocument.Parse(json);

            var items = new List<XElement>();

            if (doc.RootElement.TryGetProperty("data", out var dataArray))
            {
                foreach (var item in dataArray.EnumerateArray())
                {
                    // Strapi v5 flat format — properties are directly on the item (no attributes wrapper)
                    var title = item.TryGetProperty("title", out var t) ? t.GetString() : "Untitled";
                    var slug = item.TryGetProperty("slug", out var s) ? s.GetString() : "";
                    var excerpt = item.TryGetProperty("excerpt", out var e) ? e.GetString() : "";
                    var publishedAt = item.TryGetProperty("publishedAt", out var p) ? p.GetString() : null;

                    var link = $"https://www.trashmob.eco/news/{slug}";

                    var itemElement = new XElement("item",
                        new XElement("title", title),
                        new XElement("link", link),
                        new XElement("description", excerpt),
                        new XElement("guid", new XAttribute("isPermaLink", "true"), link));

                    if (publishedAt != null && DateTime.TryParse(publishedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var pubDate))
                    {
                        itemElement.Add(new XElement("pubDate", pubDate.ToString("R")));
                    }

                    items.Add(itemElement);
                }
            }

            var rss = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel",
                        new XElement("title", "TrashMob.eco News"),
                        new XElement("link", "https://www.trashmob.eco/news"),
                        new XElement("description", "Stories, updates, and highlights from the TrashMob community."),
                        new XElement("language", "en-us"),
                        items)));

            TrackEvent(nameof(GetNewsFeed));
            return Content(rss.Declaration + rss.ToString(), "application/rss+xml");
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
            if (string.IsNullOrWhiteSpace(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured");
            }

            TrackEvent(nameof(GetAdminUrl));
            return Ok(new { adminUrl = $"{strapiBaseUrl}/admin" });
        }
    }
}
