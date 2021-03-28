namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventAttendeeRepository
    {
        Task<IEnumerable<EventAttendee>> GetAllEventAttendees(Guid eventId);

        Task<int> AddAttendeeToEvent(Guid eventId, string attendeeId);

        Task<int> UpdateEventAttendee(EventAttendee eventAttendee);
    }
}
