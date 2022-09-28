namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnerrequests")]
    public class PartnerRequestsController : BaseController
    {
        private readonly IPartnerRequestManager partnerRequestManager;

        public PartnerRequestsController(TelemetryClient telemetryClient,
                                         IUserRepository userRepository,
                                         IPartnerRequestManager partnerRequestManager)
            : base(telemetryClient, userRepository)
        {
            this.partnerRequestManager = partnerRequestManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerRequest(PartnerRequest partnerRequest)
        {
            var user = await GetUser();

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await partnerRequestManager.Add(partnerRequest).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerRequest));

            return Ok();
        }

        [HttpPut("approve/{partnerRequestId}")]
        public async Task<IActionResult> ApprovePartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var user = await GetUser();

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestManager.ApproveBecomeAPartner(partnerRequestId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ApprovePartnerRequest));

            return Ok();
        }

        [HttpPut("deny/{partnerRequestId}")]
        public async Task<IActionResult> DenyPartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var user = await GetUser();

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestManager.DenyBecomeAPartner(partnerRequestId, cancellationToken).ConfigureAwait(false);
            
            TelemetryClient.TrackEvent(nameof(DenyPartnerRequest));

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerRequests()
        {
            var user = await GetUser();

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            return Ok(partnerRequestManager.Get());
        }

        [HttpGet("{partnerRequestId}")]
        public async Task<IActionResult> GetPartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var user = await GetUser();

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            return Ok(await partnerRequestManager.Get(partnerRequestId, cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("byuserid/{userId}")]
        public async Task<IActionResult> GetPartnerRequestsByUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetUser();

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            return Ok(await partnerRequestManager.GetByUserId(userId, cancellationToken).ConfigureAwait(false));
        }
    }
}
