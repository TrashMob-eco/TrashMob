namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;

    public interface IActiveDirectoryManager
    {
        Task<ActiveDirectoryResponseBase> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> DeleteUserAsync(Guid objectId, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> ValidateNewUserAsync(ActiveDirectoryValidateNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> UpdateUserProfileAsync(ActiveDirectoryUpdateUserProfileRequest activeDirectoryUpdateUserProfileRequest, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> ValidateUpdateUserProfileAsync(ActiveDirectoryUpdateUserProfileRequest activeDirectoryUpdateUserProfileRequest, CancellationToken cancellationToken = default);
    }
}
