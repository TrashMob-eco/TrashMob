#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class PartnerNote : KeyedModel
    {
        public Guid PartnerId { get; set; }

        public string Notes { get; set; }

        public bool IsPublic { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
