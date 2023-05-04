namespace TrashMob.Shared.Extensions
{
    using System.Collections.Generic;
    using TrashMob.Models;

    public class EventComparer : IEqualityComparer<Event>
    {
        public bool Equals(Event e1, Event e2)
        {
            return e1.Id == e2.Id;
        }

        public int GetHashCode(Event e)
        {
            return e.Id.GetHashCode();
        }
    }
}
