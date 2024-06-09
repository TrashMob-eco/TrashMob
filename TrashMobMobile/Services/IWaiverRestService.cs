namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IWaiverRestService
    {
        public Task<Waiver> GetWaiver(string waiverName, CancellationToken cancellationToken = default);
    }
}