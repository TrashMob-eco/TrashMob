﻿namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Authentication;

    public interface IUserRestService
    {
        Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByEmailAsync(string email, UserContext userContext, CancellationToken cancellationToken = default);

        Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);

        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}