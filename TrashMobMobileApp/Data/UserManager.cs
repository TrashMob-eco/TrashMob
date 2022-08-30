namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class UserManager : IUserManager
    {
        private readonly IUserRestService userRestService;

        public UserManager(IUserRestService service)
        {
            userRestService = service;
        }

        public Task<User> GetUserAsync(string userId)
        {
            return userRestService.GetUserAsync(userId);
        }

        public Task<User> AddUserAsync(User user)
        {
            return userRestService.AddUserAsync(user);
        }

        public Task<User> UpdateUserAsync(User user)
        {
            return userRestService.UpdateUserAsync(user);
        }
    }
}

