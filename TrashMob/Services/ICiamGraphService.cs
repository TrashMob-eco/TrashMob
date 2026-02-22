namespace TrashMob.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Service for querying user information from the CIAM (Entra External ID) directory via Microsoft Graph API.
    /// Used when token claims don't contain the email (common in CIAM tenants).
    /// </summary>
    public interface ICiamGraphService
    {
        /// <summary>
        /// Gets a user's email address from the CIAM directory by their object ID.
        /// </summary>
        /// <param name="objectId">The user's object ID in the CIAM directory.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user's email address, or null if not found.</returns>
        Task<string> GetUserEmailAsync(Guid objectId, CancellationToken cancellationToken = default);
    }
}
