namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventLitterReportRestService
    {
        Task<IEnumerable<FullEventLitterReport>> GetEventLitterReportsAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<FullEventLitterReport> GetEventLitterReportByLitterReportIdAsync(Guid litterReportId,
            CancellationToken cancellationToken = default);

        Task AddLitterReportAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken = default);

        Task RemoveLitterReportAsync(EventLitterReport eventLitterReport, CancellationToken cancellationToken = default);
    }
}