namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
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
            await contactRequestRepository.AddContactRequest(contactRequest);
            var message = $"From Email: {contactRequest.Email}\nFrom Name:{contactRequest.Name}\nMessage:\n{contactRequest.Message}";
            var subject = "Contact Request";
            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendGenericSystemEmail(subject, message, recipients, CancellationToken.None).ConfigureAwait(false);
            
            return Ok();
        }
    }
}
