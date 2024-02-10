namespace TrashMobMobile.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IContactRequestRestService
    {
        Task AddContactRequest(ContactRequest contactRequest, CancellationToken cancellationToken = default);
    }
}