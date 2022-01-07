namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken = default);

        Task<User> AddUser(User user);

        Task<User> UpdateUser(User user);

        Task<User> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default);

        Task<User> GetUserByUserName(string userName, CancellationToken cancellationToken = default);

        Task<User> GetUserByNameIdentifier(string nameIdentifier, CancellationToken cancellationToken = default);

        Task<int> DeleteUserByInternalId(Guid id);
    }
}
