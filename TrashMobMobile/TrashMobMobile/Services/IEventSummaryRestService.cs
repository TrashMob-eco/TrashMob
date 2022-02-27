namespace TrashMobMobile.Services
{
    using System;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IEventSummaryRestService
    {
        Task<EventSummary> GetEventSummaryAsync(Guid eventId);

        Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary);

        Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary);
    }
}