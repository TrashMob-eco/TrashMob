#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a photo flag.
    /// </summary>
    public class PhotoFlagDto
    {
        /// <summary>Gets or sets the unique identifier.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the photo identifier.</summary>
        public Guid PhotoId { get; set; }

        /// <summary>Gets or sets the photo type.</summary>
        public string PhotoType { get; set; } = string.Empty;

        /// <summary>Gets or sets the user who flagged the photo.</summary>
        public Guid FlaggedByUserId { get; set; }

        /// <summary>Gets or sets the reason for flagging.</summary>
        public string FlagReason { get; set; } = string.Empty;

        /// <summary>Gets or sets when the photo was flagged.</summary>
        public DateTimeOffset FlaggedDate { get; set; }

        /// <summary>Gets or sets when the flag was resolved.</summary>
        public DateTimeOffset? ResolvedDate { get; set; }

        /// <summary>Gets or sets the admin who resolved the flag.</summary>
        public Guid? ResolvedByUserId { get; set; }

        /// <summary>Gets or sets the resolution outcome.</summary>
        public string? Resolution { get; set; }
    }
}
