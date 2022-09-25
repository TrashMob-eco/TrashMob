#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System;

    public abstract class KeyedModel : BaseModel
    {
        public Guid Id { get; set; }
    }
}
