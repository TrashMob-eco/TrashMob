namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [Route("api/contactrequest")]
    public class ContactRequestController : BaseController
    {
        private readonly IContactRequestRepository contactRequestRepository;
        private readonly IEmailManager emailManager;

        public ContactRequestController(IContactRequestRepository contactRequestRepository, 
                                        IEmailManager emailManager, 
                                        TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.contactRequestRepository = contactRequestRepository;
            this.emailManager = emailManager;
        }

        [HttpPost]
        public async Task<IActionResult> SaveContactRequest(ContactRequest contactRequest)
        {
            await contactRequestRepository.AddContactRequest(contactRequest).ConfigureAwait(false);

            var message = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.ContactRequestReceived.ToString());
            var subject = "A Contact Request has been received on TrashMob.eco!";

            message = message.Replace("{UserName}", contactRequest.Name);
            message = message.Replace("{UserEmail}", contactRequest.Email);
            message = message.Replace("{Message}", contactRequest.Message);

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

            TelemetryClient.TrackEvent(nameof(SaveContactRequest));

            return Ok();
        }
    }
}
