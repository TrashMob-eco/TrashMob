namespace TrashMob.Shared.Managers.Prospects
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    public interface IPipelineAnalyticsManager
    {
        /// <summary>
        /// Gets pipeline analytics including funnel metrics, outreach effectiveness, and conversion rates.
        /// </summary>
        Task<PipelineAnalytics> GetAnalyticsAsync(CancellationToken cancellationToken = default);
    }
}
