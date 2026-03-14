#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class GrantDto
    {
        public Guid Id { get; set; }
        public string FunderName { get; set; } = string.Empty;
        public string? ProgramName { get; set; }
        public string? Description { get; set; }
        public decimal? AmountMin { get; set; }
        public decimal? AmountMax { get; set; }
        public decimal? AmountAwarded { get; set; }
        public int Status { get; set; }
        public DateTimeOffset? SubmissionDeadline { get; set; }
        public DateTimeOffset? AwardDate { get; set; }
        public DateTimeOffset? ReportingDeadline { get; set; }
        public DateTimeOffset? RenewalDate { get; set; }
        public Guid? FunderContactId { get; set; }
        public string? GrantUrl { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
