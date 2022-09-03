namespace TrashMobMobileApp.Data
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IEventSummaryRestService
    {
        Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default);

        Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default);
    }
}