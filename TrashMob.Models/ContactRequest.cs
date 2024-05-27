#nullable disable

namespace TrashMob.Models
{
    public class ContactRequest : KeyedModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }
    }
}