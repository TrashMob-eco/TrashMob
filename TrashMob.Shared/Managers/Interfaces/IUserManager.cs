namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IUserManager : IKeyedManager<User>
    {
        Task<User> GetUserByUserName(string userName, CancellationToken cancellationToken);

        Task<User> GetUserByInternalId(Guid id, CancellationToken cancellationToken);

        Task<bool> UserExists(Guid id);

        Task<User> UserExists(string nameIdentifier);
    }
}
