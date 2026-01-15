using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrashMob.Models;
using TrashMob.Security;
using TrashMob.Shared.Managers.Interfaces;

namespace TrashMob.Controllers;

/// <summary>
/// Controller for managing job opportunities, including CRUD operations.
/// </summary>
[Route("api/jobopportunities")]
public class JobOpportunitiesController : KeyedController<JobOpportunity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobOpportunitiesController"/> class.
    /// </summary>
    /// <param name="jobOpportunityManager">The job opportunity manager.</param>
    public JobOpportunitiesController(IKeyedManager<JobOpportunity> jobOpportunityManager)
        : base(jobOpportunityManager)
    {
    }

    /// <summary>
    /// Gets a job opportunity by its unique identifier.
    /// </summary>
    /// <param name="jobOpportunityId">The job opportunity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>The job opportunity.</remarks>
    [HttpGet("{jobOpportunityId}")]
    public async Task<IActionResult> Get(Guid jobOpportunityId, CancellationToken cancellationToken)
    {
        return Ok(await Manager.GetAsync(jobOpportunityId, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Updates a job opportunity. Admin only.
    /// </summary>
    /// <param name="jobOpportunity">The job opportunity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>The updated job opportunity.</remarks>
    [HttpPut]
    public async Task<IActionResult> Update(JobOpportunity jobOpportunity, CancellationToken cancellationToken)
    {
        var authResult = await AuthorizationService.AuthorizeAsync(User, jobOpportunity,
            AuthorizationPolicyConstants.UserIsAdmin);

        if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
        {
            return Forbid();
        }

        var result = await Manager.UpdateAsync(jobOpportunity, UserId, cancellationToken).ConfigureAwait(false);
        TelemetryClient.TrackEvent(nameof(Update) + typeof(JobOpportunity));

        return Ok(result);
    }

    /// <summary>
    /// Adds a new job opportunity. Admin only.
    /// </summary>
    /// <param name="jobOpportunity">The job opportunity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>The newly created job opportunity.</remarks>
    [HttpPost]
    public override async Task<IActionResult> Add(JobOpportunity jobOpportunity, CancellationToken cancellationToken)
    {
        var authResult = await AuthorizationService.AuthorizeAsync(User, jobOpportunity,
            AuthorizationPolicyConstants.UserIsAdmin);

        if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
        {
            return Forbid();
        }

        var result = await Manager.AddAsync(jobOpportunity, UserId, cancellationToken).ConfigureAwait(false);
        TelemetryClient.TrackEvent(nameof(Add) + typeof(JobOpportunity));

        return Ok(result);
    }

    /// <summary>
    /// Deletes a job opportunity by its unique identifier. Admin only.
    /// </summary>
    /// <param name="jobOpportunityId">The job opportunity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>The result of the delete operation.</remarks>
    [HttpDelete("{jobOpportunityId}")]
    public override async Task<IActionResult> Delete(Guid jobOpportunityId, CancellationToken cancellationToken)
    {
        var authResult = await AuthorizationService.AuthorizeAsync(User, jobOpportunityId,
            AuthorizationPolicyConstants.UserIsAdmin);

        if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
        {
            return Forbid();
        }

        var result = await Manager.DeleteAsync(jobOpportunityId, cancellationToken).ConfigureAwait(false);
        TelemetryClient.TrackEvent(nameof(Add) + typeof(JobOpportunity));

        return Ok(result);
    }
}