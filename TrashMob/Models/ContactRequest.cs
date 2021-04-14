#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class ContactRequest
    {
        public ContactRequest()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
    }
}
