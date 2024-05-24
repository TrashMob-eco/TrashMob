namespace TrashMobMobile.Data
{
    using TrashMobMobile.Models;

    public interface IWaiverManager
    {
        public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest,
            CancellationToken cancellationToken = default);

        public Task<bool> HasUserSignedTrashMobWaiverAsync(CancellationToken cancellationToken = default);
    }
}