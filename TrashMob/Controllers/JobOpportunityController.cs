namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/jobopportunities")]
    public class JobOpportunitiesController : KeyedController<JobOpportunity>
    {
        public JobOpportunitiesController(IKeyedManager<JobOpportunity> jobOpportunityManager)
            : base(jobOpportunityManager)
        {
        }

        [HttpGet("{jobOpportunityId}")]
        public async Task<IActionResult> Get(Guid jobOpportunityId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetAsync(jobOpportunityId, cancellationToken).ConfigureAwait(false));
        }

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
}
