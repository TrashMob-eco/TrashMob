namespace TrashMob.Models.Poco
{
    public class DisplayEventPartnerLocationService
    {
        public Guid EventId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int ServiceTypeId { get; set; }

        public bool IsAdvanceNoticeRequired { get; set; }

        public string PartnerLocationServicePublicNotes { get; set; } = string.Empty;

        public string PartnerName { get; set; } = string.Empty;

        public string PartnerLocationName { get; set; } = string.Empty;

        public int EventPartnerLocationServiceStatusId { get; set; }
    }
}