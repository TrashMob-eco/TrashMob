#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System;

    public partial class PartnerDocument : KeyedModel
    {
        public PartnerDocument()
        {
        }

        public Guid PartnerId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
