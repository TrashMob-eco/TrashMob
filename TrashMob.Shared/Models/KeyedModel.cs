#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public abstract class KeyedModel : BaseModel
    {
        public Guid Id { get; set; }
    }
}
