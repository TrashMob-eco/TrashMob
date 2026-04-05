namespace TrashMobMobile.Services
{
    /// <summary>
    /// Provides cached access to PRIVO feature permissions for minor users.
    /// </summary>
    public interface IPrivoPermissionService
    {
        /// <summary>
        /// Gets the cached PRIVO permissions for the current user. Fetches from API if cache is expired.
        /// Returns null for non-minor users.
        /// </summary>
        Task<Dictionary<string, string>?> GetPermissionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a specific PRIVO feature is enabled for the current user.
        /// Returns true for adult users (no restrictions) or if the feature state is "on".
        /// </summary>
        /// <param name="featureIdentifier">The PRIVO feature identifier (e.g., "trashmobservice_team").</param>
        bool IsFeatureEnabled(string featureIdentifier);

        /// <summary>
        /// Clears the cached permissions (e.g., on logout).
        /// </summary>
        void ClearCache();
    }
}
