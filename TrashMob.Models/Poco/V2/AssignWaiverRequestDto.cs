#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API request body for assigning a waiver to a community.
    /// </summary>
    public class AssignWaiverRequestDto
    {
        /// <summary>Gets or sets the waiver version ID to assign.</summary>
        public Guid WaiverId { get; set; }
    }
}
