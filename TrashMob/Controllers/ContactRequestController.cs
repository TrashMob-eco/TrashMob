namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Common;
    using TrashMob.Models;
    using TrashMob.Persistence;
    using TrashMob.Poco;

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
            var email = new Email
            {
                Message = $"From Email: {contactRequest.Email}\nFrom Name:{contactRequest.Name}\nMessage:\n{contactRequest.Message}",
                Subject = "Contact Request"
            };
            email.Addresses.Add(new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress });
           
            await emailManager.SendSystemEmail(email, CancellationToken.None).ConfigureAwait(false);
            
            return Ok();
        }
    }
}
