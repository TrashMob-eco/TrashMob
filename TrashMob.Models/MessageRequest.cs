#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    public partial class MessageRequest : KeyedModel
    {
        public MessageRequest()
        {
        }

        public string Name { get; set; }

        public string Message { get; set; }
    }
}
