namespace TrashMob.Shared.Persistence
{
    public interface ISecretRepository
    {
        string GetSecret(string name);
    }
}
