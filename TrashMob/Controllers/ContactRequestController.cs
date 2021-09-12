namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/contactrequest")]
    public class ContactRequestController : ControllerBase
    {
        private readonly IContactRequestRepository contactRequestRepository;
        private readonly IEmailManager emailManager;

        public ContactRequestController(IContactRequestRepository contactRequestRepository, IEmailManager emailManager)
        {
            this.contactRequestRepository = contactRequestRepository;
            this.emailManager = emailManager;
        }

        [HttpPost]
        public async Task<IActionResult> SaveContactRequest(ContactRequest contactRequest)
        {
            await contactRequestRepository.AddContactRequest(contactRequest).ConfigureAwait(false);

            var message = emailManager.GetEmailTemplate(NotificationTypeEnum.ContactRequestReceived.ToString());
            var htmlMessage = emailManager.GetHtmlEmailTemplate(NotificationTypeEnum.ContactRequestReceived.ToString());
            var subject = "A Contact Request has been received on TrashMob.eco!";

            message = message.Replace("{UserName}", contactRequest.Name);
            htmlMessage = htmlMessage.Replace("{UserName}", contactRequest.Name);
            message = message.Replace("{UserEmail}", contactRequest.Email);
            htmlMessage = htmlMessage.Replace("{UserEmail}", contactRequest.Email);
            message = message.Replace("{Message}", contactRequest.Message);
            htmlMessage = htmlMessage.Replace("{Message}", contactRequest.Message);

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendSystemEmail(subject, message, htmlMessage, recipients, CancellationToken.None).ConfigureAwait(false);

            return Ok();
        }
    }
}
