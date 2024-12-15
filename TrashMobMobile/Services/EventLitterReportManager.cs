namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public class EventLitterReportManager : IEventLitterReportManager
    {
        private readonly IEventLitterReportRestService eventLitterReportRestService;
        private readonly ILitterReportRestService litterReportRestService;

        public EventLitterReportManager(IEventLitterReportRestService eventLitterReportRestService, ILitterReportRestService litterReportRestService)
        {
            this.eventLitterReportRestService = eventLitterReportRestService;
            this.litterReportRestService = litterReportRestService;
        }

        public async Task<IEnumerable<FullEventLitterReport>> GetEventLitterReportsAsync(Guid eventId, ImageSizeEnum imageSize, bool getImageUrls = true,
            CancellationToken cancellationToken = default)
        {
            var eventLitterReports = await eventLitterReportRestService.GetEventLitterReportsAsync(eventId, cancellationToken);

            if (getImageUrls)
            {
                foreach (var eventLitterReport in eventLitterReports)
                {
                    foreach (var litterImage in eventLitterReport.LitterReport.LitterImages)
                    {
                        litterImage.ImageURL =
                            await litterReportRestService.GetLitterImageUrlAsync(litterImage.Id, imageSize, cancellationToken);
                    }
                }
            }

            return eventLitterReports;
        }

        public Task<FullEventLitterReport> GetEventLitterReportByLitterReportIdAsync(Guid litterReportId, CancellationToken cancellationToken = default)
        {
            return eventLitterReportRestService.GetEventLitterReportByLitterReportIdAsync(litterReportId, cancellationToken);
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
                litterImage.AzureBlobURL =
                    await litterReportRestService.GetLitterImageUrlAsync(litterImage.Id, imageSize, cancellationToken);
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
    }
}