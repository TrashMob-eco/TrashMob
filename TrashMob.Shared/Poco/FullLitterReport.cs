namespace TrashMob.Shared.Poco
{
    using System;
    using System.Collections.Generic;

    public class FullLitterReport
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int LitterReportStatusId { get; set; }

        public List<FullLitterImage> LitterImages { get; set; } = new List<FullLitterImage>();

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public string CreateByUserName { get; set; }
    }
}