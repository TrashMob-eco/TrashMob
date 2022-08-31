namespace TrashMobMobileApp.Data
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IEventSummaryRestService
    {
        Task<EventSummary> GetEventSummaryAsync(Guid eventId);

        Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary);

        Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary);
    }
}