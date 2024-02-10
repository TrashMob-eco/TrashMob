namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IDocusignRestService
    {
        public Task<EnvelopeResponse> GetWaiverEnvelopeAsync(EnvelopeRequest envelopeRequest, CancellationToken cancellationToken = default);
    }
}