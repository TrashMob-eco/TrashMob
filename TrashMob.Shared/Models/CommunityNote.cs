#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityNote : KeyedModel
    {
        public Guid CommunityId { get; set; }

        public string Notes { get; set; }

        public bool IsPublic { get; set; }

        public virtual Community Community { get; set; }
    }
}
