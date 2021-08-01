namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> AddUser(User user);

        Task<int> UpdateUser(User user);

        Task<User> GetUserByInternalId(Guid id);

        Task<User> GetUserByUserName(string userName);

        Task<User> GetUserByNameIdentifier(string nameIdentifier);

        Task<int> DeleteUserByInternalId(Guid id);
    }
}
