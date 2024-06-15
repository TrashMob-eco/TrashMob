namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IContactRequestRestService
    {
        Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default);
    }
}