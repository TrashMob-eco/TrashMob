namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventSummaryRepository
    {
        Task<EventSummary> AddEventSummary(EventSummary eventSummary);

        Task<EventSummary> UpdateEventSummary(EventSummary eventSummary);

        Task<int> DeleteEventSummary(Guid eventId);

        Task<EventSummary> GetEventSummary(Guid eventId, CancellationToken cancellationToken = default);

        IQueryable<EventSummary> GetEventSummaries(CancellationToken cancellationToken = default);
    }
}
