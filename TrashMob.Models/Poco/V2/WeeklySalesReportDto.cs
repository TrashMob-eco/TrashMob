#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// One-payload weekly sales report (Project 63 Phase 2). Mirrors the layout
    /// of Cynthia's Weekly Report spreadsheet — the salesperson consumes it on
    /// <c>/siteadmin/prospects/reports/weekly</c>, and Phase 4 emails the same
    /// shape to Cynthia and the Board.
    /// </summary>
    public class WeeklySalesReportDto
    {
        /// <summary>
        /// First day of the reporting window (inclusive), typically Monday.
        /// </summary>
        public DateTimeOffset PeriodStart { get; set; }

        /// <summary>
        /// Last day of the reporting window (inclusive), typically Sunday.
        /// </summary>
        public DateTimeOffset PeriodEnd { get; set; }

        /// <summary>
        /// Count of <see cref="CommunityProspect"/> rows created in the window.
        /// </summary>
        public int ProspectsResearched { get; set; }

        /// <summary>
        /// Count of <see cref="ProspectContact"/> rows created in the window.
        /// </summary>
        public int NewContactsAdded { get; set; }

        /// <summary>
        /// <see cref="ProspectActivity"/> rows where <c>ActivityType</c> matches
        /// <see cref="ProspectActivityTypeEnum.Outreach"/> (case-insensitive).
        /// </summary>
        public int OutreachTouches { get; set; }

        /// <summary>
        /// <see cref="ProspectActivity"/> rows matching
        /// <see cref="ProspectActivityTypeEnum.FollowUp"/>.
        /// </summary>
        public int FollowUpTouches { get; set; }

        /// <summary>
        /// <see cref="ProspectActivity"/> rows matching
        /// <see cref="ProspectActivityTypeEnum.ResponseReceived"/>.
        /// </summary>
        public int Responses { get; set; }

        /// <summary>
        /// <see cref="ProspectActivity"/> rows matching
        /// <see cref="ProspectActivityTypeEnum.MeetingRequested"/>.
        /// </summary>
        public int MeetingsRequested { get; set; }

        /// <summary>
        /// <see cref="ProspectActivity"/> rows matching
        /// <see cref="ProspectActivityTypeEnum.MeetingScheduled"/>.
        /// </summary>
        public int MeetingsScheduled { get; set; }

        /// <summary>
        /// <see cref="ProspectActivity"/> rows matching
        /// <see cref="ProspectActivityTypeEnum.MeetingHeld"/>.
        /// </summary>
        public int MeetingsHeld { get; set; }

        /// <summary>
        /// De-duplicated key-objection snippets from prospects touched in the
        /// window. Rendered under "Key Municipal Feedback" on the report.
        /// </summary>
        public List<string> KeyMunicipalFeedback { get; set; } = [];

        /// <summary>
        /// De-duplicated pricing-feedback snippets from prospects touched in
        /// the window. Rendered under "Pricing / Business Model Feedback".
        /// </summary>
        public List<string> PricingFeedback { get; set; } = [];

        /// <summary>
        /// Free-text "Next Steps" section captured by the salesperson. Persisted
        /// via the Phase 4 <see cref="SalesReport"/> entity; null on read until
        /// Phase 4 ships.
        /// </summary>
        public string? NextSteps { get; set; }
    }
}
