namespace TrashMob.Shared.Managers.Prospects
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Analyzes the sentiment of prospect reply text using AI.
    /// </summary>
    public interface ISentimentAnalysisService
    {
        /// <summary>
        /// Analyzes the sentiment of a reply text.
        /// Returns "Positive", "Neutral", or "Negative".
        /// </summary>
        Task<string> AnalyzeSentimentAsync(string replyText, CancellationToken cancellationToken = default);
    }
}
