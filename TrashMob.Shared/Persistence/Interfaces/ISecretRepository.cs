namespace TrashMob.Shared.Persistence.Interfaces
{
    public interface ISecretRepository
    {
        string GetSecret(string name);
    }
}
