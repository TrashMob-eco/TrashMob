namespace TrashMob.Models
{
    /// <summary>
    /// Defines the status of a PRIVO consent request.
    /// </summary>
    public enum ConsentStatus
    {
        /// <summary>
        /// Consent request has been created but not yet completed.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Consent has been verified/approved.
        /// </summary>
        Verified = 2,

        /// <summary>
        /// Consent was denied by the parent/guardian.
        /// </summary>
        Denied = 3,

        /// <summary>
        /// Consent has expired and needs re-verification.
        /// </summary>
        Expired = 4,

        /// <summary>
        /// Consent was revoked by the parent/guardian.
        /// </summary>
        Revoked = 5,
    }
}
