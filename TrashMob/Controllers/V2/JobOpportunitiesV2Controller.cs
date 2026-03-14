namespace TrashMob.Controllers.V2
{
    using System;
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
    /// V2 controller for managing job opportunities.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/jobopportunities")]
    public class JobOpportunitiesV2Controller(
        IKeyedManager<JobOpportunity> jobOpportunityManager,
        ILogger<JobOpportunitiesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets a job opportunity by its identifier.
        /// </summary>
        /// <param name="id">The job opportunity identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The job opportunity.</returns>
        /// <response code="200">Returns the job opportunity.</response>
        /// <response code="404">Job opportunity not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JobOpportunityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetJobOpportunity requested for Id={JobOpportunityId}", id);

            var entity = await jobOpportunityManager.GetAsync(id, cancellationToken);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity.ToV2Dto());
        }

        /// <summary>
        /// Creates a new job opportunity. Requires admin privileges.
        /// </summary>
        /// <param name="dto">The job opportunity data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created job opportunity.</returns>
        /// <response code="201">Job opportunity created successfully.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(JobOpportunityDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(JobOpportunityDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateJobOpportunity requested by User={UserId}", UserId);

            var entity = dto.ToEntity();
            var created = await jobOpportunityManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing job opportunity. Requires admin privileges.
        /// </summary>
        /// <param name="id">The job opportunity identifier.</param>
        /// <param name="dto">The updated job opportunity data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated job opportunity.</returns>
        /// <response code="200">Job opportunity updated successfully.</response>
        /// <response code="404">Job opportunity not found.</response>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(JobOpportunityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, JobOpportunityDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateJobOpportunity requested for Id={JobOpportunityId} by User={UserId}", id, UserId);

            var existing = await jobOpportunityManager.GetAsync(id, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            var entity = dto.ToEntity();
            entity.Id = id;

            var updated = await jobOpportunityManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Deletes a job opportunity. Requires admin privileges.
        /// </summary>
        /// <param name="id">The job opportunity identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Job opportunity deleted successfully.</response>
        /// <response code="404">Job opportunity not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteJobOpportunity requested for Id={JobOpportunityId} by User={UserId}", id, UserId);

            var existing = await jobOpportunityManager.GetAsync(id, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            await jobOpportunityManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
