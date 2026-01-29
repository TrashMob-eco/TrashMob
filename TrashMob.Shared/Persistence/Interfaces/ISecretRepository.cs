namespace TrashMob.Shared.Persistence.Interfaces
{
    /// <summary>
    /// Defines data access operations for retrieving application secrets and configuration values.
    /// </summary>
    public interface ISecretRepository
    {
        /// <summary>
        /// Gets a secret value by its name.
        /// </summary>
        /// <param name="name">The name of the secret to retrieve.</param>
        /// <returns>The secret value if found; otherwise, null.</returns>
        string Get(string name);
    }
}