#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System.Collections.Generic;

    /// <summary>
    /// V2 DTO for sending a test newsletter email.
    /// </summary>
    public class TestSendNewsletterDto
    {
        /// <summary>Gets or sets the list of email addresses to send the test to.</summary>
        public List<string> Emails { get; set; } = [];
    }
}
