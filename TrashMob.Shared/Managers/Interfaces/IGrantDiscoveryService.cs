namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Discovers grant opportunities using the Anthropic Claude API.
    /// </summary>
    public interface IGrantDiscoveryService
    {
        /// <summary>
        /// Discovers potential grant funding opportunities using AI.
        /// </summary>
        Task<GrantDiscoveryResult> DiscoverGrantsAsync(GrantDiscoveryRequest request, CancellationToken cancellationToken = default);
    }
}
