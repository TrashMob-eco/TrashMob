#nullable disable

namespace TrashMob.Models
{
    public class IftttTrigger : BaseModel
    {
        public string TriggerId { get; set; }

        public Guid UserId { get; set; }

        public string TriggerFields { get; set; }

        public string IftttSource { get; set; }

        public int Limit { get; set; }
    }
}