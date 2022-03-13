#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class UserWaiver
    {
        public Guid UserId { get; set; }

        public Guid WaiverId { get; set; }

        public DateTimeOffset EffectiveDate { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual Waiver Waiver { get; set; }

        public virtual User User { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
