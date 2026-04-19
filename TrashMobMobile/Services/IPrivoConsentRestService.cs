namespace TrashMobMobile.Services
{
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// REST service for PRIVO consent management API endpoints.
    /// </summary>
    public interface IPrivoConsentRestService
    {
        /// <summary>
        /// Initiates adult identity verification (Flow 1).
        /// </summary>
        Task<ParentalConsentDto> InitiateAdultVerificationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates parent-to-child consent for a 13-17 dependent (Flow 2).
        /// </summary>
        Task<ParentalConsentDto> InitiateChildConsentAsync(Guid dependentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current user's verification/consent status.
        /// </summary>
        Task<ParentalConsentDto?> GetVerificationStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the PRIVO feature permissions for the current minor user. Returns null if not a minor.
        /// </summary>
        Task<Dictionary<string, string>?> GetMinorPermissionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes consent for a specific consent record.
        /// </summary>
        Task RevokeConsentAsync(Guid consentId, string reason, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates a child-driven consent request (Flow 3). No authentication required.
        /// Returns null if the parent account was not found (204).
        /// Throws InvalidOperationException with "PARENT_NOT_VERIFIED" if parent exists but is not verified.
        /// </summary>
        Task<ParentalConsentDto?> InitiateChildInitiatedConsentAsync(
            InitiateChildConsentRequest request,
            CancellationToken cancellationToken = default);
    }
}
