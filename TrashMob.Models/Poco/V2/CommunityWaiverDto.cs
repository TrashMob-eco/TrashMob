#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a community waiver assignment.
    /// </summary>
    public class CommunityWaiverDto
    {
        /// <summary>Gets or sets the unique identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the community (partner) ID.</summary>
        public Guid CommunityId { get; set; }

        /// <summary>Gets or sets the waiver version ID.</summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>Gets or sets whether this waiver is required.</summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>Gets or sets when the assignment was created.</summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
