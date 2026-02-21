namespace TrashMob.Shared.Poco
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Summary statistics for waiver compliance dashboard.
    /// </summary>
    public class WaiverComplianceSummary
    {
        /// <summary>
        /// Gets or sets the total number of active users.
        /// </summary>
        public int TotalActiveUsers { get; set; }

        /// <summary>
        /// Gets or sets the number of users with valid waivers.
        /// </summary>
        public int UsersWithValidWaivers { get; set; }

        /// <summary>
        /// Gets or sets the number of users with expiring waivers (within 30 days).
        /// </summary>
        public int UsersWithExpiringWaivers { get; set; }

        /// <summary>
        /// Gets or sets the number of users without any valid waiver.
        /// </summary>
        public int UsersWithoutWaivers { get; set; }

        /// <summary>
        /// Gets or sets the total number of signed waivers.
        /// </summary>
        public int TotalSignedWaivers { get; set; }

        /// <summary>
        /// Gets or sets the number of waivers signed via e-signature.
        /// </summary>
        public int ESignatureCount { get; set; }

        /// <summary>
        /// Gets or sets the number of waivers uploaded as paper.
        /// </summary>
        public int PaperUploadCount { get; set; }

        /// <summary>
        /// Gets or sets the number of waivers for minors.
        /// </summary>
        public int MinorWaiversCount { get; set; }

        /// <summary>
        /// Gets or sets the compliance percentage (users with valid waivers / total active users).
        /// </summary>
        public decimal CompliancePercentage { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when this summary was generated.
        /// </summary>
        public DateTimeOffset GeneratedAt { get; set; }
    }

    /// <summary>
    /// Detailed user waiver record for admin viewing.
    /// </summary>
    public class UserWaiverDetail
    {
        /// <summary>
        /// Gets or sets the user waiver ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the waiver version ID.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the waiver name.
        /// </summary>
        public string WaiverName { get; set; }

        /// <summary>
        /// Gets or sets the waiver version string.
        /// </summary>
        public string WaiverVersion { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets when the waiver was accepted.
        /// </summary>
        public DateTimeOffset AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets when the waiver expires.
        /// </summary>
        public DateTimeOffset ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the signing method (ESignatureWeb, ESignatureMobile, PaperUpload).
        /// </summary>
        public string SigningMethod { get; set; }

        /// <summary>
        /// Gets or sets whether this was signed by a minor.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian name if signed by a minor.
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets whether the waiver is currently valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the IP address at signing (for audit).
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the document URL if available.
        /// </summary>
        public string DocumentUrl { get; set; }
    }

    /// <summary>
    /// Filter parameters for querying user waivers.
    /// </summary>
    public class UserWaiverFilter
    {
        /// <summary>
        /// Gets or sets the page number (1-based).
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// Gets or sets the waiver version ID filter.
        /// </summary>
        public Guid? WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the signing method filter.
        /// </summary>
        public string SigningMethod { get; set; }

        /// <summary>
        /// Gets or sets whether to filter for valid waivers only.
        /// </summary>
        public bool? IsValid { get; set; }

        /// <summary>
        /// Gets or sets whether to filter for minor waivers only.
        /// </summary>
        public bool? IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the minimum accepted date filter.
        /// </summary>
        public DateTimeOffset? AcceptedDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the maximum accepted date filter.
        /// </summary>
        public DateTimeOffset? AcceptedDateTo { get; set; }

        /// <summary>
        /// Gets or sets the minimum expiry date filter.
        /// </summary>
        public DateTimeOffset? ExpiryDateFrom { get; set; }

        /// <summary>
        /// Gets or sets the maximum expiry date filter.
        /// </summary>
        public DateTimeOffset? ExpiryDateTo { get; set; }

        /// <summary>
        /// Gets or sets a search term to filter by user name or email.
        /// </summary>
        public string SearchTerm { get; set; }
    }

    /// <summary>
    /// Paginated result of user waivers.
    /// </summary>
    public class UserWaiverListResult
    {
        /// <summary>
        /// Gets or sets the list of user waiver details.
        /// </summary>
        public List<UserWaiverDetail> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets the total count of matching records.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    /// <summary>
    /// Export record for CSV/legal export.
    /// </summary>
    public class WaiverExportRecord
    {
        /// <summary>
        /// Gets or sets the user waiver ID.
        /// </summary>
        public string UserWaiverId { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets the waiver name.
        /// </summary>
        public string WaiverName { get; set; }

        /// <summary>
        /// Gets or sets the waiver version.
        /// </summary>
        public string WaiverVersion { get; set; }

        /// <summary>
        /// Gets or sets when the waiver was accepted.
        /// </summary>
        public string AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets when the waiver expires.
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the signing method.
        /// </summary>
        public string SigningMethod { get; set; }

        /// <summary>
        /// Gets or sets whether this is a minor.
        /// </summary>
        public string IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian name.
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian relationship.
        /// </summary>
        public string GuardianRelationship { get; set; }

        /// <summary>
        /// Gets or sets the IP address at signing.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent at signing.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the document URL.
        /// </summary>
        public string DocumentUrl { get; set; }
    }
}
