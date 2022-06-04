namespace TrashMob.Shared.Persistence
{
    public interface IKeyVaultManager
    {
        public string GetSecret(string secretName);
    }
}
