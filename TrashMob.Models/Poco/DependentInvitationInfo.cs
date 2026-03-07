namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Information about a dependent invitation returned by the public token verification endpoint.
    /// </summary>
    public class DependentInvitationInfo
    {
        /// <summary>
        /// Gets or sets the display name of the parent who sent the invitation.
        /// </summary>
        public string ParentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the dependent being invited.
        /// </summary>
        public string DependentFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the invitation token is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets an error message if the token is invalid.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
