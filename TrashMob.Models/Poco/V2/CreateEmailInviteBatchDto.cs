namespace TrashMob.Models.Poco.V2
{
    using System.Collections.Generic;

    /// <summary>
    /// Write DTO for creating a new email invite batch.
    /// </summary>
    public class CreateEmailInviteBatchDto
    {
        /// <summary>Gets or sets the list of email addresses to invite.</summary>
        public IEnumerable<string> Emails { get; set; } = [];
    }
}
