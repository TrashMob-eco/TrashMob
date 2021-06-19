namespace TrashMob.Shared.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IContactRequestRepository
    {
        Task AddContactRequest(ContactRequest contactRequest);
    }
}
