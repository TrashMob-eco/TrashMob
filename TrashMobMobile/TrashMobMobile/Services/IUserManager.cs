namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IUserManager
    {
        Task<User> AddUserAsync(User user);
    }
}