#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class AppealRequestDto
    {
        public Guid ContactId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
