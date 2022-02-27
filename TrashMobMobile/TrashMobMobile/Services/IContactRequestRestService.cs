namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IContactRequestRestService
    {
        Task AddContactRequest(ContactRequest contactRequest);
    }
}