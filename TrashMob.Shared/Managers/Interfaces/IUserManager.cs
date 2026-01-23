namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing users.
    /// </summary>
    public interface IUserManager : IKeyedManager<User>
    {
        /// <summary>
        /// Gets a user by their name identifier.
        /// </summary>
        /// <param name="nameIdentifier">The name identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user with the specified name identifier, or null if not found.</returns>
        Task<User> GetUserByNameIdentifierAsync(string nameIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user with the specified username, or null if not found.</returns>
        Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user with the specified email address, or null if not found.</returns>
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by their internal identifier.
        /// </summary>
        /// <param name="id">The internal unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user with the specified internal identifier, or null if not found.</returns>
        Task<User> GetUserByInternalIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user by their object identifier.
        /// </summary>
        /// <param name="id">The object unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user with the specified object identifier, or null if not found.</returns>
        Task<User> GetUserByObjectIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a user exists by their identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a user exists by their name identifier.
        /// </summary>
        /// <param name="nameIdentifier">The name identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user if they exist; otherwise, null.</returns>
        Task<User> UserExistsAsync(string nameIdentifier, CancellationToken cancellationToken = default);
    }
}
