namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing attendee-submitted event metrics.
    /// </summary>
    public interface IEventAttendeeMetricsManager : IKeyedManager<EventAttendeeMetrics>
    {
        /// <summary>
        /// Submits metrics for the current user as an event attendee.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID submitting metrics.</param>
        /// <param name="metrics">The metrics data to submit.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the created or updated metrics.</returns>
        Task<ServiceResult<EventAttendeeMetrics>> SubmitMetricsAsync(
            Guid eventId,
            Guid userId,
            EventAttendeeMetrics metrics,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current user's metrics submission for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user's metrics submission, or null if not found.</returns>
        Task<EventAttendeeMetrics> GetMyMetricsAsync(
            Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all metrics submissions for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of all metrics submissions for the event.</returns>
        Task<IEnumerable<EventAttendeeMetrics>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets pending metrics submissions for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of pending metrics submissions for the event.</returns>
        Task<IEnumerable<EventAttendeeMetrics>> GetPendingByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves a metrics submission.
        /// </summary>
        /// <param name="metricsId">The metrics ID.</param>
        /// <param name="reviewerId">The user ID approving the submission.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the approved metrics.</returns>
        Task<ServiceResult<EventAttendeeMetrics>> ApproveAsync(
            Guid metricsId,
            Guid reviewerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rejects a metrics submission.
        /// </summary>
        /// <param name="metricsId">The metrics ID.</param>
        /// <param name="reason">The reason for rejection.</param>
        /// <param name="reviewerId">The user ID rejecting the submission.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the rejected metrics.</returns>
        Task<ServiceResult<EventAttendeeMetrics>> RejectAsync(
            Guid metricsId,
            string reason,
            Guid reviewerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adjusts a metrics submission with new values.
        /// </summary>
        /// <param name="metricsId">The metrics ID.</param>
        /// <param name="adjustedValues">The adjusted values.</param>
        /// <param name="reason">The reason for adjustment.</param>
        /// <param name="reviewerId">The user ID adjusting the submission.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the adjusted metrics.</returns>
        Task<ServiceResult<EventAttendeeMetrics>> AdjustAsync(
            Guid metricsId,
            EventAttendeeMetrics adjustedValues,
            string reason,
            Guid reviewerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves all pending submissions for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="reviewerId">The user ID approving the submissions.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the count of approved submissions.</returns>
        Task<ServiceResult<int>> ApproveAllPendingAsync(
            Guid eventId,
            Guid reviewerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates the totals from approved submissions for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>Aggregated totals from approved submissions.</returns>
        Task<AttendeeMetricsTotals> CalculateTotalsAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user has already submitted metrics for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the user has submitted metrics; otherwise, false.</returns>
        Task<bool> HasSubmittedMetricsAsync(
            Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
