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
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 admin controller for grant management.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/grants")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class GrantsV2Controller(
        IGrantManager grantManager,
        IGrantDiscoveryService grantDiscoveryService,
        ILogger<GrantsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all grants, optionally filtered by status.
        /// </summary>
        /// <param name="status">Optional status filter. Pass -1 or omit for all grants.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of grants.</returns>
        /// <response code="200">Returns grants.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<GrantDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int status = -1, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAll Grants Status={Status} by User={UserId}", status, UserId);

            IEnumerable<Grant> grants;

            if (status >= 0)
            {
                grants = await grantManager.GetByStatusAsync(status, cancellationToken);
            }
            else
            {
                grants = await grantManager.GetAsync(g => true, cancellationToken);
            }

            return Ok(grants.Select(g => g.ToV2Dto()));
        }

        /// <summary>
        /// Gets a grant by its identifier.
        /// </summary>
        /// <param name="id">The grant identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The grant.</returns>
        /// <response code="200">Returns the grant.</response>
        /// <response code="404">Grant not found.</response>
        [HttpGet("{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(GrantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetById Grant={GrantId} by User={UserId}", id, UserId);

            var grant = await grantManager.GetAsync(id, cancellationToken);

            if (grant is null)
            {
                return NotFound();
            }

            return Ok(grant.ToV2Dto());
        }

        /// <summary>
        /// Creates a new grant.
        /// </summary>
        /// <param name="grantDto">The grant to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created grant.</returns>
        /// <response code="201">Grant created.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(GrantDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(GrantDto grantDto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Create Grant by User={UserId}", UserId);

            var entity = grantDto.ToEntity();
            entity.CreatedByUserId = UserId;
            entity.CreatedDate = DateTimeOffset.UtcNow;
            entity.LastUpdatedByUserId = UserId;
            entity.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await grantManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing grant.
        /// </summary>
        /// <param name="grantDto">The grant to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated grant.</returns>
        /// <response code="200">Grant updated.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(GrantDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(GrantDto grantDto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Update Grant={GrantId} by User={UserId}", grantDto.Id, UserId);

            var entity = grantDto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            entity.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await grantManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Discovers potential grant funding opportunities using AI.
        /// </summary>
        /// <param name="request">The grant discovery request parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Discovered grant opportunities.</returns>
        /// <response code="200">Returns discovered grants.</response>
        [HttpPost("discover")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(GrantDiscoveryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Discover([FromBody] GrantDiscoveryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Discover Grants by User={UserId}", UserId);

            var result = await grantDiscoveryService.DiscoverGrantsAsync(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a grant.
        /// </summary>
        /// <param name="id">The grant identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Grant deleted.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Delete Grant={GrantId} by User={UserId}", id, UserId);

            await grantManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
