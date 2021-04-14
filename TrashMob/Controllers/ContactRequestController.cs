namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Persistence;

    [ApiController]
    [Route("api/contactrequest")]
    public class ContactRequestController : ControllerBase
    {
        private readonly IContactRequestRepository contactRequestRepository;

        public ContactRequestController(IContactRequestRepository contactRequestRepository)
        {
            this.contactRequestRepository = contactRequestRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SaveContactRequest(ContactRequest contactRequest)
        {
            await contactRequestRepository.AddContactRequest(contactRequest);
            return Ok();
        }
    }
}
