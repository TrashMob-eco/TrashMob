namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Areas;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for community discovery, sub-resources, and admin operations.
    /// Communities are partners with enabled home pages.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities")]
    public class CommunitiesV2Controller(
        ICommunityManager communityManager,
        IImageManager imageManager,
        INominatimService nominatimService,
        IAuthorizationService authorizationService,
        ILogger<CommunitiesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        // ============================================================================
        // Public Endpoints
        // ============================================================================

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
        /// Checks if a community slug is available.
        /// </summary>
        /// <param name="slug">The slug to check.</param>
        /// <param name="excludePartnerId">Optional partner ID to exclude (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the slug is available, false otherwise.</returns>
        /// <response code="200">Returns whether the slug is available.</response>
        [HttpGet("check-slug")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckSlug(
            [FromQuery] string slug,
            [FromQuery] Guid? excludePartnerId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CheckSlug requested Slug={Slug}, ExcludePartnerId={ExcludePartnerId}",
                slug, excludePartnerId);

            if (string.IsNullOrWhiteSpace(slug))
            {
                return Ok(false);
            }

            var isAvailable = await communityManager.IsSlugAvailableAsync(slug, excludePartnerId, cancellationToken);
            return Ok(isAvailable);
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
        /// Gets litter reports in a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of litter reports in the community.</returns>
        /// <response code="200">Returns the litter report list.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("{slug}/litterreports")]
        [ProducesResponseType(typeof(IReadOnlyList<LitterReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommunityLitterReports(
            string slug,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunityLitterReports requested Slug={Slug}", slug);

            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);

            if (community is null)
            {
                return Problem(
                    detail: $"Community with slug '{slug}' not found.",
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not found");
            }

            var litterReports = await communityManager.GetCommunityLitterReportsAsync(slug, cancellationToken);
            var dtos = litterReports.Select(lr => lr.ToV2Dto()).ToList();

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

        /// <summary>
        /// Gets communities marked as featured for display on landing and home pages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of featured communities.</returns>
        /// <response code="200">Returns the featured community list.</response>
        [HttpGet("featured")]
        [ProducesResponseType(typeof(IReadOnlyList<PartnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFeaturedCommunities(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetFeaturedCommunities requested");

            var communities = await communityManager.GetFeaturedCommunitiesAsync(cancellationToken);
            var dtos = communities.Select(c => c.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets aggregate public stats across all active communities.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Aggregate statistics for all communities.</returns>
        /// <response code="200">Returns the public stats.</response>
        [HttpGet("public-stats")]
        [ProducesResponseType(typeof(CommunityPublicStats), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPublicStats(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPublicStats requested");

            var stats = await communityManager.GetPublicStatsAsync(cancellationToken);
            return Ok(stats);
        }

        // ============================================================================
        // Admin Endpoints (Require Authentication and Authorization)
        // ============================================================================

        /// <summary>
        /// Gets community details for admin editing.
        /// Requires the user to be a community admin or site admin.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The community details.</returns>
        /// <response code="200">Returns the community.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("admin/{communityId:guid}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityForAdmin(
            Guid communityId,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunityForAdmin requested CommunityId={CommunityId}", communityId);

            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            return Ok(community.ToV2Dto());
        }

        /// <summary>
        /// Gets admin dashboard data for a community.
        /// Requires the user to be a community admin or site admin.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Dashboard data including stats, recent events, and activity.</returns>
        /// <response code="200">Returns the dashboard data.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("admin/{communityId:guid}/dashboard")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(CommunityDashboard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityDashboard(
            Guid communityId,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetCommunityDashboard requested CommunityId={CommunityId}", communityId);

            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var dashboard = await communityManager.GetCommunityDashboardAsync(communityId, cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Updates community content (branding, about, contact info).
        /// Requires the user to be a community admin or site admin.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="communityDto">The updated community data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated community.</returns>
        /// <response code="200">Returns the updated community.</response>
        /// <response code="400">Community ID mismatch or invalid data.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Community not found.</response>
        [HttpPut("admin/{communityId:guid}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCommunityContent(
            Guid communityId,
            [FromBody] PartnerDto communityDto,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdateCommunityContent requested CommunityId={CommunityId}", communityId);

            if (communityDto is null || communityDto.Id != communityId)
            {
                return BadRequest("Community ID in URL must match the body.");
            }

            var existing = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (existing is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(existing, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = communityDto.ToEntity();
            var updated = await communityManager.UpdateCommunityContentAsync(entity, UserId, cancellationToken);

            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Suggests geographic bounds for a community based on its location fields.
        /// Queries the Nominatim geocoding service to derive a bounding box.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Suggested geographic bounds with optional GeoJSON polygon.</returns>
        /// <response code="200">Returns the suggested bounds.</response>
        /// <response code="400">Community has no location info or bounds could not be found.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Community not found.</response>
        [HttpGet("admin/{communityId:guid}/suggest-bounds")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(SuggestedBounds), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SuggestBounds(
            Guid communityId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SuggestBounds requested CommunityId={CommunityId}", communityId);

            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            // Build query from community location fields
            var queryParts = new List<string>();

            if (community.RegionType.HasValue
                && community.RegionType.Value == (int)RegionTypeEnum.County
                && !string.IsNullOrWhiteSpace(community.CountyName))
            {
                queryParts.Add(community.CountyName);
            }
            else if (!string.IsNullOrWhiteSpace(community.City))
            {
                queryParts.Add(community.City);
            }

            if (!string.IsNullOrWhiteSpace(community.Region))
            {
                queryParts.Add(community.Region);
            }

            if (!string.IsNullOrWhiteSpace(community.Country))
            {
                queryParts.Add(community.Country);
            }

            if (queryParts.Count == 0)
            {
                return BadRequest("Community has no location information (city, region, or country) to look up bounds.");
            }

            var query = string.Join(", ", queryParts);
            var bounds = await nominatimService.LookupBoundsWithGeometryAsync(query, cancellationToken);

            if (bounds is null)
            {
                return BadRequest($"Could not find geographic bounds for \"{query}\". Try adjusting the community's location fields.");
            }

            var result = new SuggestedBounds
            {
                North = bounds.North,
                South = bounds.South,
                East = bounds.East,
                West = bounds.West,
                CenterLatitude = (bounds.North + bounds.South) / 2.0,
                CenterLongitude = (bounds.East + bounds.West) / 2.0,
                Query = query,
                BoundaryGeoJson = bounds.GeoJson,
            };

            return Ok(result);
        }

        // ============================================================================
        // Community Branding Image Endpoints
        // ============================================================================

        /// <summary>
        /// Uploads a community logo image (resized to 200x200).
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="imageUpload">The image file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The URL of the uploaded logo.</returns>
        /// <response code="200">Returns the logo URL.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Community not found.</response>
        [HttpPost("admin/{communityId:guid}/logo")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadCommunityLogo(
            Guid communityId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadCommunityLogo requested CommunityId={CommunityId}", communityId);

            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            imageUpload.ParentId = communityId;
            imageUpload.ImageType = ImageTypeEnum.CommunityLogo;
            var url = await imageManager.UploadImageWithSizeAsync(imageUpload, 200, 200);

            community.LogoUrl = url;
            await communityManager.UpdateCommunityContentAsync(community, UserId, cancellationToken);

            return Ok(new { url });
        }

        /// <summary>
        /// Uploads a community banner image (resized to 1200x300).
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="imageUpload">The image file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The URL of the uploaded banner.</returns>
        /// <response code="200">Returns the banner URL.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Community not found.</response>
        [HttpPost("admin/{communityId:guid}/banner")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadCommunityBanner(
            Guid communityId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadCommunityBanner requested CommunityId={CommunityId}", communityId);

            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            imageUpload.ParentId = communityId;
            imageUpload.ImageType = ImageTypeEnum.CommunityBanner;
            var url = await imageManager.UploadImageWithSizeAsync(imageUpload, 1200, 300);

            community.BannerImageUrl = url;
            await communityManager.UpdateCommunityContentAsync(community, UserId, cancellationToken);

            return Ok(new { url });
        }

        // ============================================================================
        // Private Helpers
        // ============================================================================

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
