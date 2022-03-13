#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class Waiver
    {
        public Waiver()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
        
        public DateTimeOffset EffectiveDate { get; set; }

        public int WaiverDurationTypeId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual WaiverDurationType WaiverDurationType { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
