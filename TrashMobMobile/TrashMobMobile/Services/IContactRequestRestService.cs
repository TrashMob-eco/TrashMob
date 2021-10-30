using System.Threading.Tasks;
using TrashMobMobile.Models;

namespace TrashMobMobile.Services
{
    public interface IContactRequestRestService
    {
        Task AddContactRequest(ContactRequest contactRequest);
    }
}