namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Request to send outreach emails to multiple prospects.
    /// </summary>
    public class BatchOutreachRequest
    {
        /// <summary>Gets or sets the list of prospect identifiers to send outreach to.</summary>
        public List<Guid> ProspectIds { get; set; } = [];
    }
}
