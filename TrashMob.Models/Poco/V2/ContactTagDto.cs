#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class ContactTagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
