namespace TrashMob.Shared.Persistence
{
    using TrashMob.Shared;

    public interface IDocusignManager
    {
        EnvelopeResponse SendEnvelope(EnvelopeRequest envelopeRequest);
    }
}
