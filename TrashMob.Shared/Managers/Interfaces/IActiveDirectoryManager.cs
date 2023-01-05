namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;

    public interface IActiveDirectoryManager
    {
        Task<ActiveDirectoryResponseBase> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> ValidateNewUserAsync(ActiveDirectoryValidateNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default);
    }
}
