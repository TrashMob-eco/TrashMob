namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;

    public class FullLitterReport
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int LitterReportStatusId { get; set; } = 1;

        public List<FullLitterImage> LitterImages { get; set; } = new List<FullLitterImage>();

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public string CreateByUserName { get; set; } = string.Empty;
    }
}