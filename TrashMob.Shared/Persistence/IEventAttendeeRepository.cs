namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventAttendeeRepository
    {
        Task<IEnumerable<User>> GetEventAttendees(Guid eventId);

        Task<EventAttendee> AddEventAttendee(Guid eventId, Guid attendeeId);

        Task<EventAttendee> UpdateEventAttendee(EventAttendee eventAttendee);

        Task<int> DeleteEventAttendee(Guid eventId, Guid attendeeId);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false);
    }
}
