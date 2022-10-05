namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IUserManager : IKeyedManager<User>
    {
        Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);

        Task<User> GetUserByInternalIdAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken);

        Task<User> UserExistsAsync(string nameIdentifier, CancellationToken cancellationToken);
    }
}
