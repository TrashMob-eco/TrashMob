namespace TrashMob.Controllers.V2
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Text.Json;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Security;

    /// <summary>
    /// V2 controller for proxying Strapi CMS content. Most endpoints are public.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/cms")]
    public class CmsV2Controller(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CmsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets the hero section content from the CMS.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Hero section JSON content.</returns>
        /// <response code="200">Returns hero section content.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("hero-section")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetHeroSection(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetHeroSection");

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync($"{strapiBaseUrl}/api/hero-section?populate=*", cancellationToken);
                var rewritten = RewriteMediaUrls(response, strapiBaseUrl);

                return Content(rewritten, "application/json");
            }
            catch (OperationCanceledException)
            {
                // Client disconnected — not an error worth logging
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to fetch hero section from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets the "What is TrashMob" content from the CMS.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>What is TrashMob JSON content.</returns>
        /// <response code="200">Returns content.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("what-is-trashmob")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetWhatIsTrashmob(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetWhatIsTrashmob");

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync($"{strapiBaseUrl}/api/what-is-trashmob?populate=*", cancellationToken);
                var rewritten = RewriteMediaUrls(response, strapiBaseUrl);

                return Content(rewritten, "application/json");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to fetch what-is-trashmob from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets the "Getting Started" content from the CMS.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Getting started JSON content.</returns>
        /// <response code="200">Returns content.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("getting-started")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetGettingStarted(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetGettingStarted");

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync($"{strapiBaseUrl}/api/getting-started?populate=*", cancellationToken);
                var rewritten = RewriteMediaUrls(response, strapiBaseUrl);

                return Content(rewritten, "application/json");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to fetch getting-started from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets paginated news posts from the CMS, optionally filtered by category.
        /// </summary>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Page size (1-100, default 10).</param>
        /// <param name="category">Optional category slug filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated news posts JSON.</returns>
        /// <response code="200">Returns news posts.</response>
        /// <response code="400">Invalid page size.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("news-posts")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string category = "",
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetNewsPosts Page={Page} PageSize={PageSize} Category={Category}", page, pageSize, category);

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("pageSize must be between 1 and 100.");
            }

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var url = $"{strapiBaseUrl}/api/news-posts?populate=*&sort=publishedAt:desc&pagination[page]={page}&pagination[pageSize]={pageSize}&filters[publishedAt][$notNull]=true";

                if (!string.IsNullOrEmpty(category))
                {
                    url += $"&filters[news_category][slug][$eq]={category}";
                }

                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(url, cancellationToken);
                var rewritten = RewriteMediaUrls(response, strapiBaseUrl);

                return Content(rewritten, "application/json");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to fetch news posts from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets a single news post by its slug.
        /// </summary>
        /// <param name="slug">The news post slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The news post JSON.</returns>
        /// <response code="200">Returns the news post.</response>
        /// <response code="404">News post not found.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("news-posts/{slug}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsPostBySlug(string slug, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetNewsPostBySlug Slug={Slug}", slug);

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(
                    $"{strapiBaseUrl}/api/news-posts?populate=*&filters[slug][$eq]={slug}",
                    cancellationToken);

                using var doc = JsonDocument.Parse(response);
                var dataElement = doc.RootElement.GetProperty("data");

                if (dataElement.GetArrayLength() == 0)
                {
                    return NotFound();
                }

                var rewritten = RewriteMediaUrls(response, strapiBaseUrl);

                return Content(rewritten, "application/json");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to fetch news post by slug from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets all news categories from the CMS.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>News categories JSON.</returns>
        /// <response code="200">Returns news categories.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("news-categories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsCategories(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetNewsCategories");

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync($"{strapiBaseUrl}/api/news-categories?sort=name:asc", cancellationToken);

                return Content(response, "application/json");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to fetch news categories from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets an RSS 2.0 feed of news posts.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>RSS 2.0 XML feed.</returns>
        /// <response code="200">Returns RSS feed.</response>
        /// <response code="503">CMS unavailable.</response>
        [HttpGet("news-feed")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetNewsFeed(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetNewsFeed");

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(
                    $"{strapiBaseUrl}/api/news-posts?populate=*&sort=publishedAt:desc&pagination[pageSize]=20&filters[publishedAt][$notNull]=true",
                    cancellationToken);

                using var doc = JsonDocument.Parse(response);
                var dataElement = doc.RootElement.GetProperty("data");

                var channel = new XElement("channel",
                    new XElement("title", "TrashMob News"),
                    new XElement("link", "https://www.trashmob.eco/news"),
                    new XElement("description", "Latest news from TrashMob"));

                foreach (var post in dataElement.EnumerateArray())
                {
                    var title = post.TryGetProperty("title", out var titleProp) ? titleProp.GetString() : string.Empty;
                    var slug = post.TryGetProperty("slug", out var slugProp) ? slugProp.GetString() : string.Empty;
                    var description = post.TryGetProperty("excerpt", out var excerptProp) ? excerptProp.GetString() : string.Empty;
                    var publishedAt = post.TryGetProperty("publishedAt", out var pubProp) ? pubProp.GetString() : string.Empty;
                    var postId = post.TryGetProperty("id", out var idProp) ? idProp.ToString() : string.Empty;

                    var item = new XElement("item",
                        new XElement("title", title),
                        new XElement("link", $"https://www.trashmob.eco/news/{slug}"),
                        new XElement("description", description),
                        new XElement("guid", $"https://www.trashmob.eco/news/{postId}"),
                        new XElement("pubDate", publishedAt));

                    channel.Add(item);
                }

                var rss = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("rss",
                        new XAttribute("version", "2.0"),
                        channel));

                return Content(rss.ToString(), "application/rss+xml");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Request canceled.");
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Failed to generate news feed from CMS");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS unavailable.");
            }
        }

        /// <summary>
        /// Gets the Strapi admin URL. Admin-only.
        /// </summary>
        /// <returns>The admin URL.</returns>
        /// <response code="200">Returns the admin URL.</response>
        /// <response code="503">CMS not configured.</response>
        [HttpGet("admin-url")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public IActionResult GetAdminUrl()
        {
            logger.LogInformation("V2 GetAdminUrl");

            var strapiBaseUrl = configuration["StrapiBaseUrl"];

            if (string.IsNullOrEmpty(strapiBaseUrl))
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "CMS not configured.");
            }

            return Ok(new { adminUrl = strapiBaseUrl + "/admin" });
        }

        private static string RewriteMediaUrls(string json, string strapiBaseUrl)
        {
            return json.Replace("/uploads/", $"{strapiBaseUrl}/uploads/");
        }
    }
}
