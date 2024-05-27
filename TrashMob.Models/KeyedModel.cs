#nullable disable

namespace TrashMob.Models
{
    public abstract class KeyedModel : BaseModel
    {
        public Guid Id { get; set; }
    }
}