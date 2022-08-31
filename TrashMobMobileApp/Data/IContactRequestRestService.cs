namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IContactRequestRestService
    {
        Task AddContactRequest(ContactRequest contactRequest);
    }
}