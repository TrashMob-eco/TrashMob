#nullable disable

namespace TrashMob.Models
{
    public class EventStatus : LookupModel
    {
        public virtual ICollection<Event> Events { get; set; } = [];
    }
}