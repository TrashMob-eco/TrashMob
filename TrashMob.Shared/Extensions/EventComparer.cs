namespace TrashMob.Shared.Extensions
{
    using System.Collections.Generic;
    using TrashMob.Models;

    /// <summary>
    /// Compares two Event objects for equality based on their Id property.
    /// </summary>
    public class EventComparer : IEqualityComparer<Event>
    {
        /// <inheritdoc />
        public bool Equals(Event e1, Event e2)
        {
            return e1.Id == e2.Id;
        }

        /// <inheritdoc />
        public int GetHashCode(Event e)
        {
            return e.Id.GetHashCode();
        }
    }
}