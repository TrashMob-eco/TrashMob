#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class ContactNoteDto
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public int NoteType { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
