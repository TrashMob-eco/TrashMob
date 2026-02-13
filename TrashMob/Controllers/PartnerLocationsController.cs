namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing partner locations, including retrieval, creation, update, and deletion.
    /// </summary>
    [Authorize]
    [Route("api/partnerlocations")]
    public class PartnerLocationsController : SecureController
    {
        private readonly IPartnerLocationManager partnerLocationManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocationsController"/> class.
        /// </summary>
        /// <param name="partnerLocationManager">The partner location manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public PartnerLocationsController(IPartnerLocationManager partnerLocationManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.partnerLocationManager = partnerLocationManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all partner locations for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner locations.</remarks>
        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerLocations(Guid partnerId, CancellationToken cancellationToken)
        {
            var results = await partnerLocationManager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(results);
        }

        /// <summary>
        /// Gets a partner location by its unique identifier.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner location.</remarks>
        [HttpGet("{partnerLocationId}")]
        public async Task<IActionResult> GetPartnerLocation(Guid partnerLocationId,
            CancellationToken cancellationToken = default)
        {
            var partnerLocation =
                (await partnerLocationManager.GetAsync(pl => pl.Id == partnerLocationId, cancellationToken))
                .FirstOrDefault();

            if (partnerLocation == null)
            {
                return NotFound();
            }

            return Ok(partnerLocation);
        }

        /// <summary>
        /// Adds a new partner location.
        /// </summary>
        /// <param name="partnerLocation">The partner location to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The newly created partner location.</remarks>
        [HttpPost]
        public async Task<IActionResult> AddPartnerLocation(PartnerLocation partnerLocation,
            CancellationToken cancellationToken)
        {
            if (partnerLocation == null)
            {
                return BadRequest("PartnerLocation cannot be null.");
            }

            if (partnerLocation.PartnerId == Guid.Empty)
            {
                return BadRequest("PartnerId is required.");
            }

            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await partnerLocationManager.AddAsync(partnerLocation, UserId, cancellationToken);
            TrackEvent(nameof(AddPartnerLocation));

            return CreatedAtAction(nameof(GetPartnerLocation), new { partnerLocationId = partnerLocation.Id }, result);
        }

        /// <summary>
        /// Updates an existing partner location.
        /// </summary>
        /// <param name="partnerLocation">The partner location to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated partner location.</remarks>
        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocation(PartnerLocation partnerLocation,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner =
                await partnerLocationManager.GetPartnerForLocationAsync(partnerLocation.Id, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationManager.UpdateAsync(partnerLocation, UserId, cancellationToken);
            TrackEvent(nameof(UpdatePartnerLocation));

            return Ok(partnerLocation);
        }

        /// <summary>
        /// Deletes a partner location by its unique identifier.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted partner location.</remarks>
        [HttpDelete("{partnerLocationId}")]
        public async Task<IActionResult> DeletePartnerLocation(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationManager.DeleteAsync(partnerLocationId, cancellationToken);
            TrackEvent(nameof(DeletePartnerLocation));

            return Ok(partnerLocationId);
        }

        /// <summary>
        /// Finds partners with locations near a specified point.
        /// </summary>
        /// <param name="latitude">The latitude of the search center.</param>
        /// <param name="longitude">The longitude of the search center.</param>
        /// <param name="radiusMiles">The search radius in miles (default: 25).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of partners with locations within the radius.</returns>
        [AllowAnonymous]
        [HttpGet("nearby")]
        [ProducesResponseType(typeof(IEnumerable<Partner>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNearbyPartners(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusMiles = 25,
            CancellationToken cancellationToken = default)
        {
            // Validate coordinates
            if (latitude < -90 || latitude > 90)
            {
                return BadRequest("Latitude must be between -90 and 90.");
            }

            if (longitude < -180 || longitude > 180)
            {
                return BadRequest("Longitude must be between -180 and 180.");
            }

            if (radiusMiles <= 0 || radiusMiles > 500)
            {
                return BadRequest("Radius must be between 0 and 500 miles.");
            }

            var partners = await partnerLocationManager.GetNearbyPartnersAsync(latitude, longitude, radiusMiles, cancellationToken);
            TrackEvent(nameof(GetNearbyPartners));

            return Ok(partners);
        }
    }
}