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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// V2 controller for pledge management (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/pledges")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class PledgesV2Controller(
        IPledgeManager pledgeManager,
        ILogger<PledgesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all pledges.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of pledges.</returns>
        /// <response code="200">Returns the pledge list.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<PledgeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAllPledges");

            var pledges = await pledgeManager.GetAsync(cancellationToken);
            var dtos = pledges.Select(p => p.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a pledge by ID.
        /// </summary>
        /// <param name="id">The pledge ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The pledge.</returns>
        /// <response code="200">Returns the pledge.</response>
        /// <response code="404">Pledge not found.</response>
        [HttpGet("{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PledgeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var pledge = await pledgeManager.GetAsync(id, cancellationToken);

            if (pledge is null)
            {
                return NotFound();
            }

            return Ok(pledge.ToV2Dto());
        }

        /// <summary>
        /// Gets all pledges for a specific contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of pledges for the contact.</returns>
        /// <response code="200">Returns the pledge list.</response>
        [HttpGet("bycontact/{contactId}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<PledgeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByContactId(Guid contactId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetPledgesByContact: contactId={ContactId}", contactId);

            var pledges = await pledgeManager.GetByContactIdAsync(contactId, cancellationToken);
            var dtos = pledges.Select(p => p.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Creates a new pledge.
        /// </summary>
        /// <param name="dto">The pledge data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created pledge.</returns>
        /// <response code="201">Pledge created successfully.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PledgeDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] PledgeDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CreatePledge: contactId={ContactId}", dto.ContactId);

            var entity = dto.ToEntity();
            var result = await pledgeManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a pledge.
        /// </summary>
        /// <param name="dto">The updated pledge data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated pledge.</returns>
        /// <response code="200">Pledge updated successfully.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PledgeDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] PledgeDto dto, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UpdatePledge: id={Id}", dto.Id);

            var entity = dto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            var result = await pledgeManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a pledge.
        /// </summary>
        /// <param name="id">The pledge ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Pledge deleted successfully.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 DeletePledge: id={Id}", id);

            await pledgeManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
