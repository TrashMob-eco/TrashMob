namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Discovers community prospects using the Anthropic Claude API.
    /// </summary>
    public interface IClaudeDiscoveryService
    {
        /// <summary>
        /// Discovers potential community partners using AI.
        /// </summary>
        Task<DiscoveryResult> DiscoverProspectsAsync(DiscoveryRequest request, CancellationToken cancellationToken = default);
    }
}
