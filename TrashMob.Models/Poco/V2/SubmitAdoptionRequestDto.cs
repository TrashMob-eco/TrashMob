#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API request body for submitting a team adoption application.
    /// </summary>
    public class SubmitAdoptionRequestDto
    {
        /// <summary>Gets or sets the adoptable area being applied for.</summary>
        public Guid AdoptableAreaId { get; set; }

        /// <summary>Gets or sets optional notes from the team about their application.</summary>
        public string? ApplicationNotes { get; set; }
    }
}
