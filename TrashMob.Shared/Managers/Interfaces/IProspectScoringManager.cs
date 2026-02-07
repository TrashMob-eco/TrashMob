namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Calculates FitScores for community prospects and identifies geographic gaps.
    /// </summary>
    public interface IProspectScoringManager
    {
        /// <summary>
        /// Calculates a detailed FitScore breakdown for a single prospect.
        /// </summary>
        Task<FitScoreBreakdown> CalculateFitScoreAsync(Guid prospectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recalculates and persists FitScores for all prospects.
        /// </summary>
        /// <returns>The number of prospects updated.</returns>
        Task<int> RecalculateAllScoresAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns geographic areas that have TrashMob events but no active community partner.
        /// </summary>
        Task<IEnumerable<GeographicGap>> GetGeographicGapsAsync(CancellationToken cancellationToken = default);
    }
}
