namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IContactRequestManager
    {
        Task AddContactRequestAsync(ContactRequest contactRequest);
    }
}