namespace TrashMob.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IContactRequestRepository
    {
        Task AddContactRequest(ContactRequest contactRequest);
    }
}
