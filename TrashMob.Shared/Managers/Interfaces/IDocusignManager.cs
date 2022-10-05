namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading.Tasks;
    using TrashMob.Shared;

    public interface IDocusignManager
    {
        EnvelopeResponse SendEnvelope(EnvelopeRequest envelopeRequest);

        Task<string> GetEnvelopeStatus(string envelopeId);
    }
}
