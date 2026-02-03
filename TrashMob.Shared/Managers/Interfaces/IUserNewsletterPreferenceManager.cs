namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing user newsletter preferences.
    /// </summary>
    public interface IUserNewsletterPreferenceManager
    {
        /// <summary>
        /// Gets all newsletter preferences for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of user preferences.</returns>
        Task<IEnumerable<UserNewsletterPreference>> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a user's subscription preference for a category.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="categoryId">The category ID.</param>
        /// <param name="isSubscribed">Whether to subscribe or unsubscribe.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated preference.</returns>
        Task<UserNewsletterPreference> UpdatePreferenceAsync(Guid userId, int categoryId, bool isSubscribed, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribes a user from all newsletter categories.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task UnsubscribeAllAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes default preferences for a new user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task InitializePreferencesAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all users subscribed to a specific category.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of subscribed user IDs.</returns>
        Task<IEnumerable<Guid>> GetSubscribedUsersAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
