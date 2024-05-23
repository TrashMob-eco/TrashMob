namespace TrashMobMobile.Data
{
    using TrashMobMobile.Models;

    public interface IDocusignRestService
    {
        public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest,
            CancellationToken cancellationToken = default);
    }
}