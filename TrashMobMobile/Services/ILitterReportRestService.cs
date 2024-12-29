namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface ILitterReportRestService
    {
        Task<PaginatedList<LitterReport>> GetLitterReportsAsync(LitterReportFilter filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId,
            CancellationToken cancellationToken = default);

        Task<LitterReport> GetLitterReportAsync(Guid litterReportId, CancellationToken cancellationToken = default);

        Task<LitterReport> UpdateLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default);

        Task<LitterReport> AddLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default);

        Task<string> GetLitterImageUrlAsync(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken = default);

        Task DeleteLitterReportAsync(Guid litterReportId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate,
            DateTimeOffset endDate, CancellationToken cancellationToken = default);
    }
}