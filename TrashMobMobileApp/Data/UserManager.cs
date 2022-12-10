namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public class UserManager : IUserManager
    {
        private readonly IUserRestService userRestService;

        public UserManager(IUserRestService service)
        {
            userRestService = service;
        }

        public Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return userRestService.GetUserAsync(userId, cancellationToken);
        }

        public Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return userRestService.AddUserAsync(user, cancellationToken);
        }

        public Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return userRestService.UpdateUserAsync(user, cancellationToken);
        }
    }
}

