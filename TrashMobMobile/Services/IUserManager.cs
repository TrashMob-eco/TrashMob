namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IUserManager
    {
        User CurrentUser { get; set; }

        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByEmailAsync(string email,
            CancellationToken cancellationToken = default);

        Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);

        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}