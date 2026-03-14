#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a team adoption. Flat DTO excluding navigation properties.
    /// </summary>
    public class TeamAdoptionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the team applying for adoption.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the adoptable area.
        /// </summary>
        public Guid AdoptableAreaId { get; set; }

        /// <summary>
        /// Gets or sets the date when the application was submitted.
        /// </summary>
        public DateTimeOffset ApplicationDate { get; set; }

        /// <summary>
        /// Gets or sets optional notes from the team about their application.
        /// </summary>
        public string? ApplicationNotes { get; set; }

        /// <summary>
        /// Gets or sets the status of the adoption application (Pending, Approved, Rejected, Revoked).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the identifier of the user who reviewed the application.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date when the application was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for rejection (if rejected).
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Gets or sets the start date of the adoption period.
        /// </summary>
        public DateOnly? AdoptionStartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the adoption period.
        /// </summary>
        public DateOnly? AdoptionEndDate { get; set; }

        /// <summary>
        /// Gets or sets the date of the most recent linked cleanup event.
        /// </summary>
        public DateTimeOffset? LastEventDate { get; set; }

        /// <summary>
        /// Gets or sets the total count of cleanup events linked to this adoption.
        /// </summary>
        public int EventCount { get; set; }

        /// <summary>
        /// Gets or sets whether the team is currently compliant with cleanup requirements.
        /// </summary>
        public bool IsCompliant { get; set; }

        /// <summary>
        /// Gets or sets when the adoption was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
