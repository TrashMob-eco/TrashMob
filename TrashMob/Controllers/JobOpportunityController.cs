namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing job opportunities, including CRUD operations.
    /// </summary>
    [Route("api/jobopportunities")]
    public class JobOpportunitiesController(IKeyedManager<JobOpportunity> jobOpportunityManager)
        : KeyedController<JobOpportunity>(jobOpportunityManager)
    {

        /// <summary>
        /// Gets a job opportunity by its unique identifier.
        /// </summary>
        /// <param name="jobOpportunityId">The job opportunity ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The job opportunity.</remarks>
        [HttpGet("{jobOpportunityId}")]
        public async Task<IActionResult> Get(Guid jobOpportunityId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetAsync(jobOpportunityId, cancellationToken));
        }

        /// <summary>
        /// Updates a job opportunity. Admin only.
        /// </summary>
        /// <param name="jobOpportunity">The job opportunity to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated job opportunity.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> Update(JobOpportunity jobOpportunity, CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(jobOpportunity, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var result = await Manager.UpdateAsync(jobOpportunity, UserId, cancellationToken);
            TrackEvent(nameof(Update) + typeof(JobOpportunity));

            return Ok(result);
        }

        /// <summary>
        /// Adds a new job opportunity. Admin only.
        /// </summary>
        /// <param name="jobOpportunity">The job opportunity to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The newly created job opportunity.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public override async Task<IActionResult> Add(JobOpportunity jobOpportunity, CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(jobOpportunity, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            var result = await Manager.AddAsync(jobOpportunity, UserId, cancellationToken);
            TrackEvent(nameof(Add) + typeof(JobOpportunity));

            return Ok(result);
        }

        /// <summary>
        /// Deletes a job opportunity by its unique identifier. Admin only.
        /// </summary>
        /// <param name="jobOpportunityId">The job opportunity ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The result of the delete operation.</remarks>
        [HttpDelete("{jobOpportunityId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public override async Task<IActionResult> Delete(Guid jobOpportunityId, CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(jobOpportunityId, AuthorizationPolicyConstants.UserIsAdmin))
            {
                return Forbid();
            }

            await Manager.DeleteAsync(jobOpportunityId, cancellationToken);
            TrackEvent(nameof(Add) + typeof(JobOpportunity));

            return NoContent();
        }
    }
}
