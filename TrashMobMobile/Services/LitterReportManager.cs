namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public class LitterReportManager : ILitterReportManager
    {
        private readonly ILitterReportRestService litterReportRestService;

        public LitterReportManager(ILitterReportRestService litterReportRestService)
        {
            this.litterReportRestService = litterReportRestService;
        }

        public async Task<PaginatedList<LitterReport>> GetLitterReportsAsync(LitterReportFilter filter, ImageSizeEnum imageSize, bool getImageUrls = true,
            CancellationToken cancellationToken = default)
        {
            var litterReports = await litterReportRestService.GetLitterReportsAsync(filter, cancellationToken);

            if (getImageUrls)
            {
                foreach (var litterReport in litterReports)
                {
                    foreach (var litterImage in litterReport.LitterImages)
                    {
                        try
                        {
                            litterImage.AzureBlobURL =
                                await litterReportRestService.GetLitterImageUrlAsync(litterImage.Id, imageSize, cancellationToken);
                        }
                        catch (Exception)
                        {
                            litterImage.AzureBlobURL = string.Empty;
                        }
                    }
                }
            }

            return litterReports;
        }

        public Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            return litterReportRestService.GetUserLitterReportsAsync(userId, cancellationToken);
        }

        public async Task<LitterReport> GetLitterReportAsync(Guid litterReportId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken = default)
        {
            var litterReport = await litterReportRestService.GetLitterReportAsync(litterReportId, cancellationToken);

            foreach (var litterImage in litterReport.LitterImages)
            {
                try
                {
                    litterImage.AzureBlobURL =
                        await litterReportRestService.GetLitterImageUrlAsync(litterImage.Id, imageSize, cancellationToken);
                }
                catch (Exception)
                {
                    litterImage.AzureBlobURL = string.Empty;
                }
            }

            return litterReport;
        }

        public Task<LitterReport> UpdateLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default)
        {
            return litterReportRestService.UpdateLitterReportAsync(litterReport, cancellationToken);
        }

        public Task<LitterReport> AddLitterReportAsync(LitterReport litterReport,
            CancellationToken cancellationToken = default)
        {
            return litterReportRestService.AddLitterReportAsync(litterReport, cancellationToken);
        }

        public Task DeleteLitterReportAsync(Guid litterReportId, CancellationToken cancellationToken = default)
        {
            return litterReportRestService.DeleteLitterReportAsync(litterReportId, cancellationToken);
        }

        public Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate,
            DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            return litterReportRestService.GetLocationsByTimeRangeAsync(startDate, endDate, cancellationToken);
        }

        public Task<string> GetLitterImageUrlAsync(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken = default)
        {
            return litterReportRestService.GetLitterImageUrlAsync(litterImageId, imageSize, cancellationToken);
        }
    }
}