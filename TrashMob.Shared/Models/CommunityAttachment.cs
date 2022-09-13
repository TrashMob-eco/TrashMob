#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityAttachment : ExtendedBaseModel
    {
        public CommunityAttachment()
        {
        }

        public Guid CommunityId { get; set; }

        public string AttachmentUrl { get; set; }

        public bool? IsActive { get; set; }

        public virtual Community Community { get; set; }
    }
}
