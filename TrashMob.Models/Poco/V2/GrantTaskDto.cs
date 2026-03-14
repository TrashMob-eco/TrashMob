#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class GrantTaskDto
    {
        public Guid Id { get; set; }
        public Guid GrantId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset? CompletedDate { get; set; }
        public int SortOrder { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
