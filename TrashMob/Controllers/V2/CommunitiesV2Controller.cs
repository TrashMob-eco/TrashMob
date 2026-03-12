namespace TrashMob.Controllers.V2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for community discovery and sub-resources.
    /// Communities are partners with enabled home pages.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities")]
    public class CommunitiesV2Controller(
        ICommunityManager communityManager,
        ILogger<CommunitiesV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets enabled communities, optionally filtered by location.
        /// </summary>
        /// <param name="latitude">Optional latitude for location-based filtering.</param>
        /// <param name="longitude">Optional longitude for location-based filtering.</param>
        /// <param name="radiusMiles">Optional radius in miles. All three geo params required for filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of enabled communities.</returns>
        /// <response code="200">Returns the community list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<PartnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunities(
            [FromQuery] double? latitude = null,
            [FromQuery] double? longitude = null,
            [FromQuery] double? radiusMiles = null,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunities requested Lat={Latitude}, Lon={Longitude}, Radius={RadiusMiles}",
                latitude, longitude, radiusMiles);

            var communities = await communityManager.GetEnabledCommunitiesAsync(
                latitude, longitude, radiusMiles, cancellationToken);

            var dtos = communities.Select(c => c.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a community by its URL slug.
        /// </summary>
        /// <param name="slug">The URL-friendly slug (e.g., "seattle-wa").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The community details.</returns>
        /// <response code="200">Returns the community.</response>
        /// <response code="404">Community not found or not enabled.</response>
        [HttpGet("{slug}")]
        [ProducesResponseType(typeof(PartnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityBySlug(string slug, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCommunityBySlug requested Slug={Slug}", slug);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);

            if (community is null)
            {
                return Problem(
                    detail: $"Community with slug '{slug}' not found or not enabled.",
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not found");
            }

            return Ok(community.ToV2Dto());
        }

        /// <summary>
        /// Gets events in a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="upcomingOnly">If true, only returns upcoming events. Default: true.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of events in the community.</returns>
        /// <response code="200">Returns the event list.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("{slug}/events")]
        [ProducesResponseType(typeof(IReadOnlyList<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityEvents(
            string slug,
            [FromQuery] bool upcomingOnly = true,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunityEvents requested Slug={Slug}, UpcomingOnly={UpcomingOnly}",
                slug, upcomingOnly);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);

            if (community is null)
            {
                return Problem(
                    detail: $"Community with slug '{slug}' not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not found");
            }

            var events = await communityManager.GetCommunityEventsAsync(slug, upcomingOnly, cancellationToken);
            var dtos = events.Select(e => e.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets teams near a community by location proximity.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="radiusMiles">The radius in miles to search within. Default: 50.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of nearby teams.</returns>
        /// <response code="200">Returns the team list.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("{slug}/teams")]
        [ProducesResponseType(typeof(IReadOnlyList<TeamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityTeams(
            string slug,
            [FromQuery] double radiusMiles = 50,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunityTeams requested Slug={Slug}, RadiusMiles={RadiusMiles}",
                slug, radiusMiles);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);

            if (community is null)
            {
                return Problem(
                    detail: $"Community with slug '{slug}' not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not found");
            }

            var teams = await communityManager.GetCommunityTeamsAsync(slug, radiusMiles, cancellationToken);
            var dtos = teams.Select(t => t.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets aggregated impact stats for a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Community impact statistics.</returns>
        /// <response code="200">Returns the community stats.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("{slug}/stats")]
        [ProducesResponseType(typeof(StatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityStats(
            string slug,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunityStats requested Slug={Slug}", slug);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);

            if (community is null)
            {
                return Problem(
                    detail: $"Community with slug '{slug}' not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not found");
            }

            var stats = await communityManager.GetCommunityStatsAsync(slug, cancellationToken);

            return Ok(stats.ToV2Dto());
        }
    }
}
