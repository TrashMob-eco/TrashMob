#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a team's application to adopt an area within a community.
    /// </summary>
    /// <remarks>
    /// Team adoptions go through an approval workflow where community admins review
    /// applications and approve or reject them. Approved adoptions commit the team
    /// to cleanup the area according to the area's requirements.
    /// </remarks>
    public class TeamAdoption : KeyedModel
    {
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
        public string ApplicationNotes { get; set; }

        /// <summary>
        /// Gets or sets the status of the adoption application.
        /// Valid values: "Pending", "Approved", "Rejected", "Revoked".
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
        public string RejectionReason { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the team applying for adoption.
        /// </summary>
        public virtual Team Team { get; set; }

        /// <summary>
        /// Gets or sets the adoptable area being applied for.
        /// </summary>
        public virtual AdoptableArea AdoptableArea { get; set; }

        /// <summary>
        /// Gets or sets the user who reviewed the application.
        /// </summary>
        public virtual User ReviewedByUser { get; set; }

        #endregion
    }
}
