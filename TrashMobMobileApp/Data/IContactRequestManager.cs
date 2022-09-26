namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IContactRequestManager
    {
        Task AddContactRequestAsync(ContactRequest contactRequest, CancellationToken cancellationToken = default);
    }
}