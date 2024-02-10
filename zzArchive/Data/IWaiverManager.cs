namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Models;

    public interface IWaiverManager
    {
        public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest, CancellationToken cancellationToken = default);

        public Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default);
    }
}