namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventPartnerLocationServiceRestService
    {
        Task<PartnerLocation> GetHaulingPartnerLocationAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayEventPartnerLocation>> GetEventPartnerLocationsAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayEventPartnerLocationService>> GetEventPartnerLocationServicesAsync(Guid eventId,
            Guid partnerId, CancellationToken cancellationToken = default);

        Task<EventPartnerLocationService> UpdateEventPartnerLocationService(
            EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken = default);

        Task<EventPartnerLocationService> AddEventPartnerLocationService(
            EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken = default);

        Task DeleteEventPartnerLocationServiceAsync(EventPartnerLocationService eventPartnerLocationService,
            CancellationToken cancellationToken = default);
    }
}