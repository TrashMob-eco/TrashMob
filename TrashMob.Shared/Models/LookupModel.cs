#nullable disable

namespace TrashMob.Shared.Models
{
    public abstract class LookupModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
