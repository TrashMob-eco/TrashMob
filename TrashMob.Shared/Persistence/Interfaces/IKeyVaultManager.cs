namespace TrashMob.Shared.Persistence.Interfaces
{
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public interface IKeyVaultManager
    {
        public string GetSecret(string secretName);

        public Task<X509Certificate2> GetCertificateAsync(string certificateName);
    }
}
