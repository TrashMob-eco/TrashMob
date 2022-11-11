namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;

    public interface IActiveDirectoryManager
    {
        Task<ActiveDirectoryResponseBase> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default);

        Task<ActiveDirectoryResponseBase> ValidateUserAsync(ActiveDirectoryValidateNewUserRequest activeDirectoryNewUserRequest, CancellationToken cancellationToken = default);
    }
}
