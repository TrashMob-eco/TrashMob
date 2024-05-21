using System;

namespace TrashMob.Shared.Poco
{
    public class GeneralFilter
{
        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public Guid? CreatedByUserId { get; set; }

    }
}
