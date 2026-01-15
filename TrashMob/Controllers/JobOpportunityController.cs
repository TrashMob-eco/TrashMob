using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrashMob.Models;
using TrashMob.Security;
using TrashMob.Shared.Managers.Interfaces;

namespace TrashMob.Controllers;

/// <summary>
/// Controller for managing job opportunities, including CRUD operations.
/// </summary>
[Route("api/jobopportunities")]
[ApiController]
public class JobOpportunitiesController : ControllerBase
{
    private readonly IAuthorizationService authorizationService;
    private readonly IJobManager jobManager;
    private readonly TelemetryClient telemetryClient;
    
    private Guid UserId
    {
        get
        {
            if (HttpContext.Items.TryGetValue("UserId", out var value) &&
                value is Guid userId)
            {
                return userId;
            }

            return Guid.Empty;
        }
    }

    public JobOpportunitiesController(
        IAuthorizationService authorizationService,
        IJobManager jobManager,
        TelemetryClient telemetryClient
        )
    {
        this.authorizationService = authorizationService;
        this.jobManager = jobManager;
        this.telemetryClient = telemetryClient;
    }
    
    /// <summary>
    /// Gets a list of all job opportunities.
    /// </summary>
    /// <param name="isActive">When true, only return active jobs</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobOpportunity>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobsAsync([FromQuery] bool isActive, CancellationToken cancellationToken)
    {
        var result = await jobManager.GetJobsAsync(isActive, cancellationToken).ConfigureAwait(false);
        return Ok(result);
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
        return Ok(await jobManager.GetAsync(jobOpportunityId, cancellationToken).ConfigureAwait(false));
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
        var authResult = await authorizationService.AuthorizeAsync(User, jobOpportunity,
            AuthorizationPolicyConstants.UserIsAdmin);

        if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
        {
            return Forbid();
        }

        var result = await jobManager.UpdateAsync(jobOpportunity, UserId, cancellationToken).ConfigureAwait(false);
        telemetryClient.TrackEvent(nameof(Update) + typeof(JobOpportunity));

        return Ok(result);
    }

    /// <summary>
    /// Adds a new job opportunity. Admin only.
    /// </summary>
    /// <param name="jobOpportunity">The job opportunity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>The newly created job opportunity.</remarks>
    [HttpPost]
    public async Task<IActionResult> Add(JobOpportunity jobOpportunity, CancellationToken cancellationToken)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, jobOpportunity,
            AuthorizationPolicyConstants.UserIsAdmin);

        if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
        {
            return Forbid();
        }

        var result = await jobManager.AddAsync(jobOpportunity, UserId, cancellationToken).ConfigureAwait(false);
        telemetryClient.TrackEvent(nameof(Add) + typeof(JobOpportunity));

        return Ok(result);
    }

    /// <summary>
    /// Deletes a job opportunity by its unique identifier. Admin only.
    /// </summary>
    /// <param name="jobOpportunityId">The job opportunity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>The result of the delete operation.</remarks>
    [HttpDelete("{jobOpportunityId}")]
    public async Task<IActionResult> Delete(Guid jobOpportunityId, CancellationToken cancellationToken)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, jobOpportunityId,
            AuthorizationPolicyConstants.UserIsAdmin);

        if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
        {
            return Forbid();
        }

        var result = await jobManager.DeleteAsync(jobOpportunityId, cancellationToken).ConfigureAwait(false);
        telemetryClient.TrackEvent(nameof(Add) + typeof(JobOpportunity));

        return Ok(result);
    }
}