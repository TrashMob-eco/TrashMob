﻿namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventAttendeeManager : IBaseManager<EventAttendee>
    {
        Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(Guid attendeeId, bool futureEventsOnly = false,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(EventFilter filter, Guid attendeeId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCanceledEventsUserIsAttendingAsync(Guid attendeeId, bool futureEventsOnly = false,
            CancellationToken cancellationToken = default);
    }
}