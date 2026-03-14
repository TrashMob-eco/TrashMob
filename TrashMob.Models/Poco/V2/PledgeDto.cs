#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class PledgeDto
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int Frequency { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
