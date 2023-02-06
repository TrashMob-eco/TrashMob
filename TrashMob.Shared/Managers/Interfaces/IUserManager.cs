namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IUserManager : IKeyedManager<User>
    {
        Task<User> GetUserByNameIdentifierAsync(string nameIdentifier, CancellationToken cancellationToken = default);

        Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default);

        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<User> GetUserByInternalIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<User> GetUserByObjectIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<User> UserExistsAsync(string nameIdentifier, CancellationToken cancellationToken = default);
    }
}
