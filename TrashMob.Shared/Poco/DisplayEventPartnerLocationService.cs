
namespace TrashMob.Shared.Poco
{
    using System;

    public class DisplayEventPartnerLocationService
    {
        public Guid EventId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int ServiceTypeId { get; set; }

        public bool IsAdvanceNoticeRequired { get; set; }

        public string PartnerLocationServicePublicNotes { get; set; }

        public string PartnerName { get; set; }

        public string PartnerLocationName { get; set; }

        public int EventPartnerLocationServiceStatusId { get; set; }
    }
}
