namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Response from PRIVO /requests API (Sections 2, 5, 6).
    /// Contains the identifiers needed to track and complete a consent flow.
    /// </summary>
    public class PrivoConsentResponse
    {
        /// <summary>
        /// Gets or sets the PRIVO Service Identifier (SiD) for the principal user.
        /// </summary>
        public string Sid { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the consent identifier representing this request.
        /// </summary>
        public string ConsentIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the consent URL for redirecting the user to the PRIVO consent flow.
        /// </summary>
        public string ConsentUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the PRIVO SiD for the granter (parent), if applicable.
        /// </summary>
        public string? GranterSid { get; set; }
    }
}
