#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityDocument : KeyedModel
    {
        public CommunityDocument()
        {
        }

        public Guid CommunityId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public virtual Community Community { get; set; }
    }
}
