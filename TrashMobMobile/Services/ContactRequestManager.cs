namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public class ContactRequestManager(IContactRequestRestService service) : IContactRequestManager
    {
        private readonly IContactRequestRestService contactRequestRestService = service;

        public Task AddContactRequestAsync(ContactRequest contactRequest, CancellationToken cancellationToken = default)
        {
            return contactRequestRestService.AddContactRequest(contactRequest, cancellationToken);
        }
    }
}