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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for admin waiver version management.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin/waivers")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class WaiverAdminV2Controller(
        IWaiverVersionManager waiverVersionManager,
        ILogger<WaiverAdminV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all waiver versions.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of all waiver versions.</returns>
        /// <response code="200">Returns all waiver versions.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<WaiverVersionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAllWaiverVersions");

            var waivers = await waiverVersionManager.GetAllAsync(cancellationToken);

            return Ok(waivers.Select(w => w.ToV2Dto()));
        }

        /// <summary>
        /// Gets all active waiver versions.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of active waiver versions.</returns>
        /// <response code="200">Returns active waiver versions.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet("active")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<WaiverVersionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetActiveWaiverVersions");

            var waivers = await waiverVersionManager.GetActiveWaiversAsync(cancellationToken);

            return Ok(waivers.Select(w => w.ToV2Dto()));
        }

        /// <summary>
        /// Gets a waiver version by ID.
        /// </summary>
        /// <param name="id">The waiver version ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The waiver version.</returns>
        /// <response code="200">Returns the waiver version.</response>
        /// <response code="403">User is not an admin.</response>
        /// <response code="404">Waiver version not found.</response>
        [HttpGet("{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverVersionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetWaiverVersion WaiverId={WaiverId}", id);

            var waiver = await waiverVersionManager.GetAsync(id, cancellationToken);

            if (waiver == null)
            {
                return NotFound();
            }

            return Ok(waiver.ToV2Dto());
        }

        /// <summary>
        /// Creates a new waiver version.
        /// </summary>
        /// <param name="dto">The waiver version data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created waiver version.</returns>
        /// <response code="201">Waiver version created.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(WaiverVersionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] WaiverVersionDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateWaiverVersion Name={Name} Version={Version}", dto.Name, dto.Version);

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Waiver name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Version))
            {
                return BadRequest("Waiver version is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.WaiverText))
            {
                return BadRequest("Waiver text is required.");
            }

            var entity = new WaiverVersion
            {
                Name = dto.Name,
                Version = dto.Version,
                WaiverText = dto.WaiverText,
                IsActive = dto.IsActive,
                EffectiveDate = dto.EffectiveDate,
                Scope = dto.Scope,
                CreatedByUserId = UserId,
                LastUpdatedByUserId = UserId,
            };

            var result = await waiverVersionManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing waiver version.
        /// </summary>
        /// <param name="id">The waiver version ID.</param>
        /// <param name="dto">The updated waiver version data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated waiver version.</returns>
        /// <response code="200">Waiver version updated.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not an admin.</response>
        /// <response code="404">Waiver version not found.</response>
        [HttpPut("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(WaiverVersionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] WaiverVersionDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateWaiverVersion WaiverId={WaiverId}", id);

            var existing = await waiverVersionManager.GetAsync(id, cancellationToken);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = dto.Name;
            existing.Version = dto.Version;
            existing.WaiverText = dto.WaiverText;
            existing.IsActive = dto.IsActive;
            existing.EffectiveDate = dto.EffectiveDate;
            existing.ExpiryDate = dto.ExpiryDate;
            existing.Scope = dto.Scope;
            existing.LastUpdatedByUserId = UserId;
            existing.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await waiverVersionManager.UpdateAsync(existing, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deactivates a waiver version.
        /// </summary>
        /// <param name="id">The waiver version ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Waiver version deactivated.</response>
        /// <response code="403">User is not an admin.</response>
        /// <response code="404">Waiver version not found.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeactivateWaiverVersion WaiverId={WaiverId}", id);

            var existing = await waiverVersionManager.GetAsync(id, cancellationToken);

            if (existing == null)
            {
                return NotFound();
            }

            await waiverVersionManager.DeactivateAsync(id, UserId, cancellationToken);

            return NoContent();
        }
    }
}
