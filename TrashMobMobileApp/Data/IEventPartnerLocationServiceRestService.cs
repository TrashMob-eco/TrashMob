namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventPartnerLocationServiceRestService
    {
        Task<PartnerLocation> GetHaulingPartnerLocationAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}