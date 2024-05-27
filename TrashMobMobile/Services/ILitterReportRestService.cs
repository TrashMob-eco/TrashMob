namespace TrashMobMobile.Data
{
    using TrashMob.Models;

    public interface ILitterReportRestService
    {
        Task<IEnumerable<LitterReport>> GetAllLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>>
            GetNotCancelledLitterReportsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(CancellationToken cancellationToken = default);

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