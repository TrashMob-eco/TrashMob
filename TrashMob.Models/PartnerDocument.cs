#nullable disable

namespace TrashMob.Models
{
    public class PartnerDocument : KeyedModel
    {
        public Guid PartnerId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public virtual Partner Partner { get; set; }
    }
}