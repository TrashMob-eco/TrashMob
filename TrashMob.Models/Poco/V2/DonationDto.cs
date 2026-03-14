#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class DonationDto
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset DonationDate { get; set; }
        public int DonationType { get; set; }
        public string? Campaign { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurringFrequency { get; set; }
        public Guid? PledgeId { get; set; }
        public string? InKindDescription { get; set; }
        public string? MatchingGiftEmployer { get; set; }
        public string? Notes { get; set; }
        public bool ReceiptSent { get; set; }
        public bool ThankYouSent { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
