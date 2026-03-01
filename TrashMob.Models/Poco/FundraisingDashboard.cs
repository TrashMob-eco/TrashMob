namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Engagement score breakdown for a single contact.
    /// </summary>
    public class ContactEngagementScore
    {
        /// <summary>Gets or sets the contact identifier.</summary>
        public Guid ContactId { get; set; }

        /// <summary>Gets or sets the display name.</summary>
        public string ContactName { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the contact type.</summary>
        public int ContactType { get; set; }

        /// <summary>Gets or sets the overall engagement score (0–100).</summary>
        public int EngagementScore { get; set; }

        /// <summary>Gets or sets the auto-calculated donor lifecycle stage.</summary>
        public string DonorLifecycleStage { get; set; } = string.Empty;

        /// <summary>Gets or sets total lifetime donation amount.</summary>
        public decimal TotalDonations { get; set; }

        /// <summary>Gets or sets the number of donations.</summary>
        public int DonationCount { get; set; }

        /// <summary>Gets or sets the date of the most recent donation.</summary>
        public DateTimeOffset? LastDonationDate { get; set; }

        /// <summary>Gets or sets the number of events attended (via linked User).</summary>
        public int EventsAttended { get; set; }

        /// <summary>Gets or sets total volunteer minutes (from EventAttendeeMetrics).</summary>
        public int TotalVolunteerMinutes { get; set; }

        /// <summary>Gets or sets the number of contact notes.</summary>
        public int NoteCount { get; set; }

        /// <summary>Gets or sets the date of the most recent interaction note.</summary>
        public DateTimeOffset? LastInteractionDate { get; set; }

        /// <summary>Gets or sets whether this contact is LYBUNT (gave last year but not this year).</summary>
        public bool IsLybunt { get; set; }

        /// <summary>Gets or sets whether this contact has a linked registered user.</summary>
        public bool HasUserId { get; set; }

        /// <summary>Gets or sets the donation score component (0–40).</summary>
        public int DonationScoreComponent { get; set; }

        /// <summary>Gets or sets the volunteer score component (0–40).</summary>
        public int VolunteerScoreComponent { get; set; }

        /// <summary>Gets or sets the interaction score component (0–20).</summary>
        public int InteractionScoreComponent { get; set; }
    }

    /// <summary>
    /// Fundraising dashboard aggregate metrics.
    /// </summary>
    public class FundraisingDashboard
    {
        /// <summary>Gets or sets total raised year-to-date.</summary>
        public decimal TotalRaisedYtd { get; set; }

        /// <summary>Gets or sets total raised in the previous year.</summary>
        public decimal TotalRaisedLastYear { get; set; }

        /// <summary>Gets or sets the number of unique donors year-to-date.</summary>
        public int DonorCountYtd { get; set; }

        /// <summary>Gets or sets the average gift size year-to-date.</summary>
        public decimal AverageGiftSizeYtd { get; set; }

        /// <summary>Gets or sets the number of donations year-to-date.</summary>
        public int DonationCountYtd { get; set; }

        /// <summary>Gets or sets the donor retention rate (percent).</summary>
        public double RetentionRate { get; set; }

        /// <summary>Gets or sets the count of new donors year-to-date.</summary>
        public int NewDonorsYtd { get; set; }

        /// <summary>Gets or sets the count of repeat donors year-to-date.</summary>
        public int RepeatDonorsYtd { get; set; }

        /// <summary>Gets or sets the count of lapsed donors.</summary>
        public int LapsedDonors { get; set; }

        /// <summary>Gets or sets the donor lifecycle stage breakdown.</summary>
        public List<DonorLifecycleStat> LifecycleBreakdown { get; set; } = [];

        /// <summary>Gets or sets the monthly giving trend (last 12 months).</summary>
        public List<MonthlyGivingStat> MonthlyGiving { get; set; } = [];

        /// <summary>Gets or sets the campaign breakdown.</summary>
        public List<CampaignStat> CampaignBreakdown { get; set; } = [];

        /// <summary>Gets or sets the total grant amount awarded.</summary>
        public decimal TotalGrantsAwarded { get; set; }

        /// <summary>Gets or sets the total grant amount pending.</summary>
        public decimal TotalGrantsPending { get; set; }

        /// <summary>Gets or sets the number of active grants.</summary>
        public int ActiveGrantCount { get; set; }

        /// <summary>Gets or sets the number of grants with upcoming deadlines.</summary>
        public int UpcomingDeadlineCount { get; set; }

        /// <summary>Gets or sets the grant pipeline breakdown by status.</summary>
        public List<GrantPipelineStat> GrantPipeline { get; set; } = [];

        /// <summary>Gets or sets the LYBUNT count.</summary>
        public int LybuntCount { get; set; }
    }

    /// <summary>
    /// Donor lifecycle stage count.
    /// </summary>
    public class DonorLifecycleStat
    {
        /// <summary>Gets or sets the lifecycle stage name.</summary>
        public string Stage { get; set; } = string.Empty;

        /// <summary>Gets or sets the count of contacts in this stage.</summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// Monthly giving aggregate.
    /// </summary>
    public class MonthlyGivingStat
    {
        /// <summary>Gets or sets the month (YYYY-MM format).</summary>
        public string Month { get; set; } = string.Empty;

        /// <summary>Gets or sets the total amount donated in this month.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the number of donations in this month.</summary>
        public int DonationCount { get; set; }
    }

    /// <summary>
    /// Campaign fundraising aggregate.
    /// </summary>
    public class CampaignStat
    {
        /// <summary>Gets or sets the campaign name.</summary>
        public string Campaign { get; set; } = string.Empty;

        /// <summary>Gets or sets the total raised for this campaign.</summary>
        public decimal TotalRaised { get; set; }

        /// <summary>Gets or sets the number of unique donors for this campaign.</summary>
        public int DonorCount { get; set; }

        /// <summary>Gets or sets the number of donations for this campaign.</summary>
        public int DonationCount { get; set; }
    }

    /// <summary>
    /// Grant pipeline status aggregate.
    /// </summary>
    public class GrantPipelineStat
    {
        /// <summary>Gets or sets the grant status value.</summary>
        public int Status { get; set; }

        /// <summary>Gets or sets the display label for this status.</summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>Gets or sets the count of grants in this status.</summary>
        public int Count { get; set; }

        /// <summary>Gets or sets the total amount for grants in this status.</summary>
        public decimal TotalAmount { get; set; }
    }
}
