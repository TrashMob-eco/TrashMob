namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for community discovery and detail pages.
    /// Communities are partners with enabled home pages.
    /// </summary>
    [Route("api/communities")]
    public class CommunitiesController(ICommunityManager communityManager) : SecureController
    {
        /// <summary>
        /// Gets all communities with enabled home pages.
        /// </summary>
        /// <param name="latitude">Optional latitude for location-based filtering.</param>
        /// <param name="longitude">Optional longitude for location-based filtering.</param>
        /// <param name="radiusMiles">Optional radius in miles for location filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Partner>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommunities(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusMiles,
            CancellationToken cancellationToken)
        {
            var communities = await communityManager.GetEnabledCommunitiesAsync(
                latitude, longitude, radiusMiles, cancellationToken);
            return Ok(communities);
        }

        /// <summary>
        /// Gets a community by its URL slug.
        /// </summary>
        /// <param name="slug">The community slug (e.g., "seattle-wa").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{slug}")]
        [ProducesResponseType(typeof(Partner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityBySlug(string slug, CancellationToken cancellationToken)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            return Ok(community);
        }

        /// <summary>
        /// Checks if a community slug is available.
        /// </summary>
        /// <param name="slug">The slug to check.</param>
        /// <param name="excludePartnerId">Optional partner ID to exclude (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("check-slug")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckSlug(
            [FromQuery] string slug,
            [FromQuery] System.Guid? excludePartnerId,
            CancellationToken cancellationToken)
        {
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
        /// <param name="upcomingOnly">If true, only returns upcoming events.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{slug}/events")]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityEvents(
            string slug,
            [FromQuery] bool upcomingOnly = true,
            CancellationToken cancellationToken = default)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            var events = await communityManager.GetCommunityEventsAsync(slug, upcomingOnly, cancellationToken);
            return Ok(events);
        }

        /// <summary>
        /// Gets teams near a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="radiusMiles">The radius in miles to search within.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{slug}/teams")]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityTeams(
            string slug,
            [FromQuery] double radiusMiles = 50,
            CancellationToken cancellationToken = default)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            var teams = await communityManager.GetCommunityTeamsAsync(slug, radiusMiles, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Gets litter reports in a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{slug}/litterreports")]
        [ProducesResponseType(typeof(IEnumerable<LitterReport>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityLitterReports(
            string slug,
            CancellationToken cancellationToken = default)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            var litterReports = await communityManager.GetCommunityLitterReportsAsync(slug, cancellationToken);
            return Ok(litterReports);
        }

        /// <summary>
        /// Gets aggregated stats for a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{slug}/stats")]
        [ProducesResponseType(typeof(Stats), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityStats(
            string slug,
            CancellationToken cancellationToken = default)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            var stats = await communityManager.GetCommunityStatsAsync(slug, cancellationToken);
            return Ok(stats);
        }

        // ============================================================================
        // Admin Endpoints (Require Authentication and Authorization)
        // ============================================================================

        /// <summary>
        /// Gets admin dashboard data for a community.
        /// Requires the user to be a community admin or site admin.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("admin/{communityId:guid}/dashboard")]
        [Authorize]
        [ProducesResponseType(typeof(CommunityDashboard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCommunityDashboard(
            Guid communityId,
            CancellationToken cancellationToken = default)
        {
            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var dashboard = await communityManager.GetCommunityDashboardAsync(communityId, cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Gets community details for admin editing.
        /// Requires the user to be a community admin or site admin.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("admin/{communityId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Partner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCommunityForAdmin(
            Guid communityId,
            CancellationToken cancellationToken = default)
        {
            var community = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (community == null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(community);
        }

        /// <summary>
        /// Updates community content (branding, about, contact info).
        /// Requires the user to be a community admin or site admin.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="community">The updated community data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("admin/{communityId:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(Partner), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCommunityContent(
            Guid communityId,
            [FromBody] Partner community,
            CancellationToken cancellationToken = default)
        {
            if (community == null || community.Id != communityId)
            {
                return BadRequest("Community ID in URL must match the body.");
            }

            var existing = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, existing, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updated = await communityManager.UpdateCommunityContentAsync(community, UserId, cancellationToken);
            TrackEvent(nameof(UpdateCommunityContent));

            return Ok(updated);
        }
    }
}
