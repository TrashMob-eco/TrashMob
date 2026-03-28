namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Manages PRIVO consent workflows for adult identity verification and parental consent.
    /// </summary>
    public interface IPrivoConsentManager
    {
        /// <summary>
        /// Initiates adult identity verification via PRIVO (Flow 1).
        /// </summary>
        /// <param name="userId">The user ID of the adult to verify.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created consent record with PRIVO consent URL.</returns>
        Task<ParentalConsent> InitiateAdultVerificationAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates parent-to-child consent for a 13-17 dependent via PRIVO (Flow 2).
        /// </summary>
        /// <param name="parentUserId">The verified parent's user ID.</param>
        /// <param name="dependentId">The dependent to request consent for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created consent record with PRIVO consent URL.</returns>
        Task<ParentalConsent> InitiateParentChildConsentAsync(Guid parentUserId, Guid dependentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates a child-driven consent request (Flow 3).
        /// </summary>
        /// <param name="childFirstName">The child's first name.</param>
        /// <param name="childEmail">The child's email address.</param>
        /// <param name="childBirthDate">The child's date of birth.</param>
        /// <param name="parentEmail">The parent's email address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created consent record, or null if parent must create account first.</returns>
        Task<ParentalConsent> InitiateChildConsentAsync(string childFirstName, string childEmail, DateOnly childBirthDate, string parentEmail, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes an incoming PRIVO webhook payload.
        /// </summary>
        /// <param name="payload">The webhook payload from PRIVO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ProcessWebhookAsync(PrivoWebhookPayload payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current verification/consent status for a user.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The most recent consent record, or null if none exists.</returns>
        Task<ParentalConsent> GetConsentByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes consent for a specific consent record.
        /// </summary>
        /// <param name="consentId">The consent record ID.</param>
        /// <param name="requestingUserId">The user requesting revocation.</param>
        /// <param name="reason">The reason for revocation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RevokeConsentAsync(Guid consentId, Guid requestingUserId, string reason, CancellationToken cancellationToken = default);
    }
}
