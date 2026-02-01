namespace TrashMob.Controllers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for community discovery and detail pages.
    /// Communities are partners with enabled home pages.
    /// </summary>
    [Route("api/communities")]
    public class CommunitiesController(ICommunityManager communityManager) : BaseController
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
    }
}
