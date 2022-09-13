#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class ContactRequest : BaseModel
    {
        public ContactRequest()
        {
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
    }
}
