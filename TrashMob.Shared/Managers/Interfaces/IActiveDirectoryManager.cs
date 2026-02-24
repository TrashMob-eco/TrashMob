namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing users in Azure Active Directory.
    /// </summary>
    public interface IActiveDirectoryManager
    {
        /// <summary>
        /// Creates a new user in Active Directory.
        /// </summary>
        /// <param name="activeDirectoryNewUserRequest">The new user request details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response indicating success or failure.</returns>
        Task<ActiveDirectoryResponseBase> CreateUserAsync(ActiveDirectoryNewUserRequest activeDirectoryNewUserRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a user from Active Directory.
        /// </summary>
        /// <param name="objectId">The object ID of the user to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response indicating success or failure.</returns>
        Task<ActiveDirectoryResponseBase> DeleteUserAsync(Guid objectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates a new user request before creation.
        /// </summary>
        /// <param name="activeDirectoryNewUserRequest">The new user request to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The validation response.</returns>
        Task<ActiveDirectoryResponseBase> ValidateNewUserAsync(
            ActiveDirectoryValidateNewUserRequest activeDirectoryNewUserRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's profile in Active Directory.
        /// </summary>
        /// <param name="activeDirectoryUpdateUserProfileRequest">The profile update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response indicating success or failure.</returns>
        Task<ActiveDirectoryResponseBase> UpdateUserProfileAsync(
            ActiveDirectoryUpdateUserProfileRequest activeDirectoryUpdateUserProfileRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates a user profile update request.
        /// </summary>
        /// <param name="activeDirectoryUpdateUserProfileRequest">The profile update request to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The validation response.</returns>
        Task<ActiveDirectoryResponseBase> ValidateUpdateUserProfileAsync(
            ActiveDirectoryUpdateUserProfileRequest activeDirectoryUpdateUserProfileRequest,
            CancellationToken cancellationToken = default);
    }
}