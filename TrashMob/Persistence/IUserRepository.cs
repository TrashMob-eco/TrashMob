namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<Guid> AddUser(User user);

        Task<int> UpdateUser(User user);

        Task<User> GetUserByInternalId(Guid id);

        Task<User> GetUserByExternalId(string tenantId, string uniqueId);

        Task<int> DeleteUserByInternalId(Guid id);
    }
}
