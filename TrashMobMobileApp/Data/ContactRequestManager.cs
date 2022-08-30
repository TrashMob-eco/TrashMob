namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class ContactRequestManager : IContactRequestManager
    {
        private readonly IContactRequestRestService contactRequestRestService;

        public ContactRequestManager(IContactRequestRestService service)
        {
            contactRequestRestService = service;
        }

        public Task AddContactRequestAsync(ContactRequest contactRequest)
        {
            return contactRequestRestService.AddContactRequest(contactRequest);
        }
    }
}

