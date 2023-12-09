namespace TrashMobMobile.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public class ContactRequestManager : IContactRequestManager
    {
        private readonly IContactRequestRestService contactRequestRestService;

        public ContactRequestManager(IContactRequestRestService service)
        {
            contactRequestRestService = service;
        }

        public Task AddContactRequestAsync(ContactRequest contactRequest, CancellationToken cancellationToken = default)
        {
            return contactRequestRestService.AddContactRequest(contactRequest, cancellationToken);
        }
    }
}

