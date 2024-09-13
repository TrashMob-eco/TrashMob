namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IEventLitterReportRestService
    {
        Task<IEnumerable<EventLitterReport>> GetEventLitterReportsAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task AddLitterReportAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken = default);

        Task RemoveLitterReportAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken = default);
    }
}