namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Areas;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for community discovery and detail pages.
    /// Communities are partners with enabled home pages.
    /// </summary>
    [Route("api/communities")]
    public class CommunitiesController(
        ICommunityManager communityManager,
        IPartnerPhotoManager partnerPhotoManager,
        IImageManager imageManager,
        INominatimService nominatimService) : SecureController
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
            if (community is null)
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
            if (community is null)
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
            if (community is null)
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
            if (community is null)
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
            if (community is null)
            {
                return NotFound();
            }

            var stats = await communityManager.GetCommunityStatsAsync(slug, cancellationToken);
            return Ok(stats);
        }

        /// <summary>
        /// Gets communities marked as featured for display on landing and home pages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("featured")]
        [ProducesResponseType(typeof(IEnumerable<Partner>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeaturedCommunities(CancellationToken cancellationToken)
        {
            var communities = await communityManager.GetFeaturedCommunitiesAsync(cancellationToken);
            return Ok(communities);
        }

        /// <summary>
        /// Gets aggregate public stats across all active communities.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("public-stats")]
        [ProducesResponseType(typeof(CommunityPublicStats), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicStats(CancellationToken cancellationToken)
        {
            var stats = await communityManager.GetPublicStatsAsync(cancellationToken);
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
            if (community is null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
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
            if (community is null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
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
            if (community is null || community.Id != communityId)
            {
                return BadRequest("Community ID in URL must match the body.");
            }

            var existing = await communityManager.GetByIdAsync(communityId, cancellationToken);
            if (existing is null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            if (!await IsAuthorizedAsync(existing, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var updated = await communityManager.UpdateCommunityContentAsync(community, UserId, cancellationToken);
            TrackEvent(nameof(UpdateCommunityContent));

            return Ok(updated);
        }

        /// <summary>
        /// Suggests geographic bounds for a community based on its location fields.
        /// Queries the Nominatim geocoding service to derive a bounding box.
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("admin/{communityId:guid}/suggest-bounds")]
        [Authorize]
        [ProducesResponseType(typeof(SuggestedBounds), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SuggestBounds(
            Guid communityId,
            CancellationToken cancellationToken)
        {
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
            var bounds = await nominatimService.LookupBoundsAsync(query, cancellationToken);

            if (bounds is null)
            {
                return BadRequest($"Could not find geographic bounds for \"{query}\". Try adjusting the community's location fields.");
            }

            var result = new SuggestedBounds
            {
                North = bounds.Value.North,
                South = bounds.Value.South,
                East = bounds.Value.East,
                West = bounds.Value.West,
                CenterLatitude = (bounds.Value.North + bounds.Value.South) / 2.0,
                CenterLongitude = (bounds.Value.East + bounds.Value.West) / 2.0,
                Query = query,
            };

            TrackEvent(nameof(SuggestBounds));
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

            TrackEvent(nameof(UploadCommunityLogo));
            return Ok(new { url });
        }

        /// <summary>
        /// Uploads a community banner image (resized to 1200x300).
        /// </summary>
        /// <param name="communityId">The community/partner ID.</param>
        /// <param name="imageUpload">The image file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
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

            TrackEvent(nameof(UploadCommunityBanner));
            return Ok(new { url });
        }

        // ============================================================================
        // Community Photo Gallery Endpoints
        // ============================================================================

        #region Community Photos

        /// <summary>
        /// Gets all photos for a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{slug}/photos")]
        [ProducesResponseType(typeof(IEnumerable<PartnerPhoto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommunityPhotos(string slug, CancellationToken cancellationToken)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            var photos = await partnerPhotoManager.GetByPartnerIdAsync(community.Id, cancellationToken);
            return Ok(photos);
        }

        /// <summary>
        /// Uploads a photo for a community. Only community admins can upload photos.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{slug}/photos")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerPhoto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadCommunityPhoto(
            string slug,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            // Create the partner photo record
            var photoId = Guid.NewGuid();
            var partnerPhoto = new PartnerPhoto
            {
                Id = photoId,
                PartnerId = community.Id,
                Caption = string.Empty,
            };

            // Upload to blob storage
            imageUpload.ParentId = photoId;
            imageUpload.ImageType = ImageTypeEnum.PartnerPhoto;
            await imageManager.UploadImageAsync(imageUpload);

            // Get the image URL and save photo record
            var imageUrl = await imageManager.GetImageUrlAsync(photoId, ImageTypeEnum.PartnerPhoto, ImageSizeEnum.Reduced, cancellationToken);
            partnerPhoto.ImageUrl = imageUrl;
            var createdPhoto = await partnerPhotoManager.AddAsync(partnerPhoto, UserId, cancellationToken);

            TrackEvent(nameof(UploadCommunityPhoto));
            return CreatedAtAction(nameof(GetCommunityPhotos), new { slug }, createdPhoto);
        }

        /// <summary>
        /// Deletes a community photo. Only community admins can delete photos.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{slug}/photos/{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCommunityPhoto(
            string slug,
            Guid photoId,
            CancellationToken cancellationToken)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var photo = await partnerPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.PartnerId != community.Id)
            {
                return NotFound();
            }

            // HardDelete removes from blob storage and database
            await partnerPhotoManager.HardDeleteAsync(photoId, cancellationToken);
            TrackEvent(nameof(DeleteCommunityPhoto));
            return NoContent();
        }

        /// <summary>
        /// Updates a community photo caption. Only community admins can update photos.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="photoId">The photo ID.</param>
        /// <param name="caption">The new caption.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{slug}/photos/{photoId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerPhoto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCommunityPhotoCaption(
            string slug,
            Guid photoId,
            [FromBody] string caption,
            CancellationToken cancellationToken)
        {
            var community = await communityManager.GetBySlugAsync(slug, cancellationToken);
            if (community is null)
            {
                return NotFound();
            }

            // Authorize: user must be partner admin or site admin
            if (!await IsAuthorizedAsync(community, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var photo = await partnerPhotoManager.GetAsync(photoId, cancellationToken);
            if (photo is null || photo.PartnerId != community.Id)
            {
                return NotFound();
            }

            photo.Caption = caption ?? string.Empty;
            photo.LastUpdatedByUserId = UserId;
            photo.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedPhoto = await partnerPhotoManager.UpdateAsync(photo, UserId, cancellationToken);
            TrackEvent(nameof(UpdateCommunityPhotoCaption));
            return Ok(updatedPhoto);
        }

        #endregion
    }
}
