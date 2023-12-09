namespace TrashMobMobile.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Authentication;

    public interface IUserManager
    {
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByEmailAsync(string email, UserContext userContext, CancellationToken cancellationToken = default);

        Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);

        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}