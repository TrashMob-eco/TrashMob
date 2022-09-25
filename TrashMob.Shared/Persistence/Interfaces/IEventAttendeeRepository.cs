namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventAttendeeRepository
    {
        Task<IEnumerable<User>> GetEventAttendees(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventAttendee> AddEventAttendee(Guid eventId, Guid attendeeId);

        Task<EventAttendee> UpdateEventAttendee(EventAttendee eventAttendee);

        Task<int> DeleteEventAttendee(Guid eventId, Guid attendeeId);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCanceledEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false, CancellationToken cancellationToken = default);
    }
}
