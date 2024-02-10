namespace TrashMobMobileApp.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventPartnerLocationServiceStatusRestService
    {
        Task<IEnumerable<EventPartnerLocationServiceStatus>> GetEventPartnerLocationServiceStatusesAsync(CancellationToken cancellationToken = default);
    }
}