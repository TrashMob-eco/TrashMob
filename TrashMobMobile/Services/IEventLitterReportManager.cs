namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventLitterReportManager
    {
        Task<IEnumerable<FullEventLitterReport>> GetEventLitterReportsAsync(Guid eventId, ImageSizeEnum imageSize, bool getImageUrls = true, CancellationToken cancellationToken = default);
        
        Task<FullEventLitterReport> GetEventLitterReportByLitterReportIdAsync(Guid litterReportId, CancellationToken cancellationToken = default);
    }
}