namespace TrashMobMobileApp.Models
{
    using System;

    public class EventType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
