namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Service for communicating with the PRIVO consent management and identity verification API.
    /// Handles token management, consent requests, user info retrieval, and consent revocation.
    /// </summary>
    public interface IPrivoService
    {
        /// <summary>
        /// Creates an adult identity verification request (Section 2).
        /// </summary>
        /// <param name="user">The adult user to verify.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The consent response containing SiD, consent identifier, and consent URL.</returns>
        Task<PrivoConsentResponse> CreateAdultVerificationRequestAsync(
            User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a direct verification URL, bypassing PRIVO pre-screens (Section 3).
        /// </summary>
        /// <param name="consentIdentifier">The consent identifier from Section 2.</param>
        /// <param name="redirectUrl">The URL to redirect to after verification.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The direct verification URL.</returns>
        Task<string> GetDirectVerificationUrlAsync(
            string consentIdentifier, string redirectUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves user information and feature states (Section 4).
        /// </summary>
        /// <param name="sid">The PRIVO Service Identifier for the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user info, or null if not found.</returns>
        Task<PrivoUserInfo> GetUserInfoBySidAsync(
            string sid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user info by External ID (Section 4).
        /// </summary>
        /// <param name="eid">The external identifier (TrashMob User.Id or Dependent.Id).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user info including feature states and attributes.</returns>
        Task<PrivoUserInfo> GetUserInfoByEidAsync(
            string eid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a parent-initiated child consent request (Section 5).
        /// </summary>
        /// <param name="parent">The verified parent user.</param>
        /// <param name="child">The dependent child.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The consent response containing child SiD, consent identifier, and consent URL.</returns>
        Task<PrivoConsentResponse> CreateParentInitiatedChildConsentAsync(
            User parent, Dependent child, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a child-initiated consent request (Section 6).
        /// </summary>
        /// <param name="childFirstName">The child's first name.</param>
        /// <param name="childEmail">The child's email address.</param>
        /// <param name="childBirthDate">The child's date of birth.</param>
        /// <param name="parentEmail">The parent's email address.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The consent response containing SiDs and consent identifier.</returns>
        Task<PrivoConsentResponse> CreateChildInitiatedConsentAsync(
            string childFirstName, string childEmail, DateOnly childBirthDate,
            string parentEmail, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current consent status (Section 7).
        /// </summary>
        /// <param name="consentIdentifier">The consent identifier.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The consent status string.</returns>
        Task<string> GetConsentStatusAsync(
            string consentIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Syncs email verification status from Entra to PRIVO (Section 8).
        /// </summary>
        /// <param name="sid">The PRIVO SiD of the user whose email is verified.</param>
        /// <param name="attributeIdentifier">The PRIVO attribute identifier for the email.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task SyncEmailVerificationAsync(
            string sid, string attributeIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes consent for a user (Section 9).
        /// </summary>
        /// <param name="principalSid">The principal (child) SiD.</param>
        /// <param name="granterSid">The granter (parent) SiD.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task RevokeConsentAsync(
            string principalSid, string granterSid, CancellationToken cancellationToken = default);
    }
}
