namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    public interface IDocusignManager
    {
        EnvelopeResponse SendEnvelope(EnvelopeRequest envelopeRequest);

        Task<string> GetEnvelopeStatusAsync(string envelopeId, CancellationToken cancellationToken);
    }
}
