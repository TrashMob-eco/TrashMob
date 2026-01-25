namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a partner admin invitation for display purposes.
    /// </summary>
    public class DisplayPartnerAdminInvitation
    {
        /// <summary>
        /// Gets or sets the unique identifier of the invitation.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the partner associated with the invitation.
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;
    }
}