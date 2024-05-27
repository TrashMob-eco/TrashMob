#nullable disable

namespace TrashMob.Models
{
    public class MessageRequest : KeyedModel
    {
        public string Name { get; set; }

        public string Message { get; set; }
    }
}