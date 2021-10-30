using System.Threading.Tasks;
using TrashMobMobile.Models;

namespace TrashMobMobile.Services
{
    public interface IContactRequestManager
    {
        Task AddContactRequestAsync(ContactRequest contactRequest);
    }
}