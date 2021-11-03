namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventSummaryRepository
    {
        Task<EventSummary> AddEventSummary(EventSummary eventSummary);

        Task<EventSummary> UpdateEventSummary(EventSummary eventSummary);

        Task<int> DeleteEventSummary(Guid eventId);

        Task<EventSummary> GetEventSummary(Guid eventId);

        IQueryable<EventSummary> GetEventSummaries();
    }
}
