namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerrequests")]
    public class PartnerRequestsController : SecureController
    {
        private readonly IPartnerRequestManager partnerRequestManager;

        public PartnerRequestsController(IPartnerRequestManager partnerRequestManager) : base()
        {
            this.partnerRequestManager = partnerRequestManager;
        }

        [HttpPost]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> AddPartnerRequest(PartnerRequest partnerRequest, CancellationToken cancellationToken)
        {
            await partnerRequestManager.AddAsync(partnerRequest, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerRequest));

            return Ok();
        }

        [HttpPut("approve/{partnerRequestId}")]
        [Authorize(Policy = "UserIsAdmin")]
        public async Task<IActionResult> ApprovePartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var partnerRequest = await partnerRequestManager.ApproveBecomeAPartnerAsync(partnerRequestId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ApprovePartnerRequest));

            return Ok();
        }

        [HttpPut("deny/{partnerRequestId}")]
        [Authorize(Policy = "UserIsAdmin")]
        public async Task<IActionResult> DenyPartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var partnerRequest = await partnerRequestManager.DenyBecomeAPartnerAsync(partnerRequestId, cancellationToken).ConfigureAwait(false);
            
            TelemetryClient.TrackEvent(nameof(DenyPartnerRequest));

            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = "UserIsAdmin")]
        public async Task<IActionResult> GetPartnerRequests(CancellationToken cancellationToken)
        {
            return Ok(await partnerRequestManager.GetAsync(cancellationToken));
        }

        [HttpGet("{partnerRequestId}")]
        [Authorize(Policy = "UserIsAdmin")]
        public async Task<IActionResult> GetPartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            return Ok(await partnerRequestManager.GetAsync(partnerRequestId, cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("byuserid/{userId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnerRequestsByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await partnerRequestManager.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false));
        }
    }
}
