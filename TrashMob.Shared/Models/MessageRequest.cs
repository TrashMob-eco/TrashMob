#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class MessageRequest
    {
        public MessageRequest()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
    }
}
