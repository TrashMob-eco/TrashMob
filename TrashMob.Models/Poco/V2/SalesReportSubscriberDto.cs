#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a distribution-list entry for the weekly and
    /// monthly sales pipeline emails (Project 63 Phase 4b).
    /// </summary>
    public class SalesReportSubscriberDto
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the subscribed user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the subscribed user's display name (populated on read).
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the subscribed user's email (populated on read).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives the weekly email.
        /// </summary>
        public bool IncludeWeekly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives the monthly email.
        /// </summary>
        public bool IncludeMonthly { get; set; }
    }

    /// <summary>
    /// Request body for adding a subscriber.
    /// </summary>
    public class AddSalesReportSubscriberRequest
    {
        /// <summary>
        /// Gets or sets the user id to subscribe.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives the weekly email.
        /// </summary>
        public bool IncludeWeekly { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives the monthly email.
        /// </summary>
        public bool IncludeMonthly { get; set; } = true;
    }

    /// <summary>
    /// Request body for updating an existing subscription's cadence flags.
    /// </summary>
    public class UpdateSalesReportSubscriberRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives the weekly email.
        /// </summary>
        public bool IncludeWeekly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the subscriber receives the monthly email.
        /// </summary>
        public bool IncludeMonthly { get; set; }
    }
}
