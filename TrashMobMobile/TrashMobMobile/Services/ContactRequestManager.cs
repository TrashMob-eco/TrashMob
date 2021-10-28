namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class ContactRequestManager
    {
        private readonly ContactRequestRestService contactRequestRestService;

        public ContactRequestManager(ContactRequestRestService service)
        {
            contactRequestRestService = service;
        }

        public Task AddContactRequestAsync(ContactRequest contactRequest)
        {
            return contactRequestRestService.AddContactRequest(contactRequest);
        }
    }
}

