namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Generates AI-personalized outreach email content for community prospects.
    /// </summary>
    public interface IOutreachContentService
    {
        /// <summary>
        /// Generates personalized outreach email content for a prospect at a specific cadence step.
        /// </summary>
        /// <param name="prospect">The community prospect to generate content for.</param>
        /// <param name="cadenceStep">The cadence step (1=Initial, 2=Follow-up, 3=Value-add, 4=Final).</param>
        /// <param name="nearbyEventCount">Number of TrashMob events near this prospect's location.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An outreach preview with the generated subject and HTML body.</returns>
        Task<OutreachPreview> GenerateOutreachContentAsync(CommunityProspect prospect, int cadenceStep,
            int nearbyEventCount, CancellationToken cancellationToken = default);
    }
}
