namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnerrequests")]
    public class PartnerRequestsController : BaseController
    {
        private readonly IKeyedManager<PartnerRequest> partnerRequestManager;
        private readonly IEmailManager emailManager;

        public PartnerRequestsController(TelemetryClient telemetryClient,
                                         IUserRepository userRepository,
                                         IKeyedManager<PartnerRequest> partnerRequestManager,
                                         IEmailManager emailManager)
            : base(telemetryClient, userRepository)
        {
            this.partnerRequestManager = partnerRequestManager;
            this.emailManager = emailManager;
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

            var message = $"From Email: {partnerRequest.Email}\nFrom Name:{partnerRequest.Name}\nMessage:\n{partnerRequest.Notes}";
            var subject = "Partner Request";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                emailCopy = message,
                subject = subject,
            };

            await emailManager.SendTemplatedEmail(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            return Ok();
        }

        [HttpPut("approve/{partnerRequestId}")]
        public async Task<IActionResult> ApprovePartnerRequest(Guid partnerRequestId)
        {
            var user = await GetUser();

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestManager.Get(partnerRequestId).ConfigureAwait(false);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Approved;

            await partnerRequestManager.Update(partnerRequest).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ApprovePartnerRequest));

            await partnerRequestManager.Add(partnerRequest).ConfigureAwait(false);

            var partnerMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.PartnerRequestAccepted.ToString());
            partnerMessage = partnerMessage.Replace("{PartnerName}", partnerRequest.Name);
            var partnerSubject = "Your request to become a TrashMob.eco Partner has been accepted!";

            var dynamicTemplateData = new
            {
                username = partnerRequest.Name,
                emailCopy = partnerMessage,
                subject = partnerSubject,
            };

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = partnerRequest.Name, Email = partnerRequest.Email },
            };
            
            await emailManager.SendTemplatedEmail(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return Ok();
        }

        [HttpPut("deny/{partnerRequestId}")]
        public async Task<IActionResult> DenyPartnerRequest(Guid partnerRequestId)
        {
            var user = await GetUser();

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestManager.Get(partnerRequestId).ConfigureAwait(false);
            
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Denied;

            await partnerRequestManager.Update(partnerRequest).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DenyPartnerRequest));

            var partnerMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.PartnerRequestDeclined.ToString());
            partnerMessage = partnerMessage.Replace("{PartnerName}", partnerRequest.Name);
            var partnerSubject = "Your request to become a TrashMob.eco Partner has been declined";

            var dynamicTemplateData = new
            {
                username = partnerRequest.Name,
                emailCopy = partnerMessage,
                subject = partnerSubject,
            };

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = partnerRequest.Name, Email = partnerRequest.Email },
            };

            await emailManager.SendTemplatedEmail(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerRequests(CancellationToken cancellationToken)
        {
            var user = await GetUser();

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            return Ok(partnerRequestManager.Get());
        }

        [HttpGet("{userId}")]
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
