#nullable enable

namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Distribution-list management for the weekly and monthly sales pipeline
    /// emails (Project 63 Phase 4b). SiteAdmin-only from the API side; the
    /// scheduled job reads via <see cref="GetForCadenceAsync"/>.
    /// </summary>
    public interface ISalesReportSubscriberService
    {
        /// <summary>
        /// Returns all subscribers with their <see cref="SalesReportSubscriber.User"/>
        /// navigation populated, ordered by user name.
        /// </summary>
        Task<IReadOnlyCollection<SalesReportSubscriber>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns subscribers currently opted in to the given cadence.
        /// Used by the hourly job to build the recipient list.
        /// </summary>
        Task<IReadOnlyCollection<SalesReportSubscriber>> GetForCadenceAsync(
            SalesReportPeriodTypeEnum periodType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new subscription. If the user is already subscribed, updates
        /// the cadence flags in place instead of throwing.
        /// </summary>
        Task<SalesReportSubscriber> AddOrUpdateAsync(
            Guid userId,
            bool includeWeekly,
            bool includeMonthly,
            Guid actingUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates cadence flags on an existing subscription by id.
        /// Returns null when the subscription does not exist.
        /// </summary>
        Task<SalesReportSubscriber?> UpdateAsync(
            Guid subscriptionId,
            bool includeWeekly,
            bool includeMonthly,
            Guid actingUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a subscription. Returns false when no matching row exists.
        /// </summary>
        Task<bool> DeleteAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    }
}
