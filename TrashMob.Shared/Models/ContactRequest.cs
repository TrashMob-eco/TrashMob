#nullable disable

namespace TrashMob.Shared.Models
{
    public partial class ContactRequest : KeyedModel
    {
        public ContactRequest()
        {
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }
    }
}
