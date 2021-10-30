namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IUserRestService
    {
        Task<User> AddUser(User user);
    }
}