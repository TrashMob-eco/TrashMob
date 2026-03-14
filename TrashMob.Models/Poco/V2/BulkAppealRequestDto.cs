#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;
    public class BulkAppealRequestDto
    {
        public List<Guid> ContactIds { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
