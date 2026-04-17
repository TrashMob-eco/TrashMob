namespace TrashMobMobile.Services
{
    /// <summary>
    /// Caches PRIVO feature permissions for minor users in-memory with a 1-hour TTL.
    /// </summary>
    public class PrivoPermissionService(IPrivoConsentRestService privoRestService) : IPrivoPermissionService
    {
        private Dictionary<string, string>? cachedPermissions;
        private DateTime cacheExpiry = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        /// <inheritdoc />
        public async Task<Dictionary<string, string>?> GetPermissionsAsync(CancellationToken cancellationToken = default)
        {
            if (cachedPermissions != null && DateTime.UtcNow < cacheExpiry)
            {
                return cachedPermissions;
            }

            cachedPermissions = await privoRestService.GetMinorPermissionsAsync(cancellationToken);
            cacheExpiry = DateTime.UtcNow.Add(CacheDuration);
            return cachedPermissions;
        }

        /// <inheritdoc />
        public bool IsFeatureEnabled(string featureIdentifier)
        {
            if (cachedPermissions == null)
            {
                return true; // Default to enabled if no permissions loaded (adult user)
            }

            return cachedPermissions.TryGetValue(featureIdentifier, out var state)
                && string.Equals(state, "on", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public void ClearCache()
        {
            cachedPermissions = null;
            cacheExpiry = DateTime.MinValue;
        }
    }
}
