namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IEventPartnerLocationServiceStatusRestService
    {
        Task<IEnumerable<EventPartnerLocationServiceStatus>> GetEventPartnerLocationServiceStatusesAsync(
            CancellationToken cancellationToken = default);
    }
}