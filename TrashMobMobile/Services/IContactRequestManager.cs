namespace TrashMobMobile.Data
{
    using TrashMob.Models;

    public interface IContactRequestManager
    {
        Task AddContactRequestAsync(ContactRequest contactRequest, CancellationToken cancellationToken = default);
    }
}