#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class MessageRequest : BaseModel
    {
        public MessageRequest()
        {
        }

        public string Name { get; set; }

        public string Message { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
    }
}
