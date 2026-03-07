namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Request body for creating a dependent invitation.
    /// </summary>
    public class CreateDependentInvitationRequest
    {
        /// <summary>
        /// Gets or sets the email address to send the invitation to.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
