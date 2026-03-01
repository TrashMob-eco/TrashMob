#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a grant opportunity in the pipeline.
    /// </summary>
    public class Grant : KeyedModel
    {
        public Grant()
        {
            GrantTasks = [];
        }

        public string FunderName { get; set; }

        public string ProgramName { get; set; }

        public string Description { get; set; }

        public decimal? AmountMin { get; set; }

        public decimal? AmountMax { get; set; }

        public decimal? AmountAwarded { get; set; }

        public int Status { get; set; }

        public DateTimeOffset? SubmissionDeadline { get; set; }

        public DateTimeOffset? AwardDate { get; set; }

        public DateTimeOffset? ReportingDeadline { get; set; }

        public DateTimeOffset? RenewalDate { get; set; }

        public Guid? FunderContactId { get; set; }

        public string GrantUrl { get; set; }

        public string Notes { get; set; }

        public virtual Contact FunderContact { get; set; }

        public virtual ICollection<GrantTask> GrantTasks { get; set; }
    }
}
