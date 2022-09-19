namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Authorize]
    [Route("api/partnerrequests")]
    public class PartnerRequestsController : BaseController
    {
        private readonly IPartnerRequestRepository partnerRequestRepository;
        private readonly IPartnerManager partnerManager;
        private readonly IUserRepository userRepository;
        private readonly IEmailManager emailManager;

        public PartnerRequestsController(IPartnerRequestRepository partnerRequestRepository, 
                                         IPartnerManager partnerManager,
                                         IEmailManager emailManager, 
                                         IUserRepository userRepository,
                                         TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.partnerRequestRepository = partnerRequestRepository;
            this.partnerManager = partnerManager;
            this.emailManager = emailManager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerRequest(PartnerRequest partnerRequest)
        {
            var user = await userRepository.GetUserByInternalId(partnerRequest.CreatedByUserId).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await partnerRequestRepository.AddPartnerRequest(partnerRequest).ConfigureAwait(false);
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
            var user = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestRepository.GetPartnerRequest(partnerRequestId).ConfigureAwait(false);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Approved;

            await partnerRequestRepository.UpdatePartnerRequest(partnerRequest).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ApprovePartnerRequest));

            await partnerManager.CreatePartner(partnerRequest).ConfigureAwait(false);

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
            var user = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestRepository.GetPartnerRequest(partnerRequestId).ConfigureAwait(false);
            
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Denied;

            await partnerRequestRepository.UpdatePartnerRequest(partnerRequest).ConfigureAwait(false);
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
            var user = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value, cancellationToken).ConfigureAwait(false);

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            return Ok(await partnerRequestRepository.GetPartnerRequests(cancellationToken).ConfigureAwait(false));
        }
    }
}
