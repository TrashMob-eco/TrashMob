namespace TrashMob.Shared.Persistence
{
    using Azure.Security.KeyVault.Secrets;
    using System.Threading.Tasks;

    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly SecretClient secretClient;

        public KeyVaultManager(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        public string GetSecret(string secretName)
        {
            var keyValueSecret = secretClient.GetSecret(secretName);

            return keyValueSecret.Value.Value;
        }
    }
}
