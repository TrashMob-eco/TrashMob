namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IUserManager
    {
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);

        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}