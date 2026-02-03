namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for attendee-submitted event metrics operations.
    /// </summary>
    public class EventAttendeeMetricsManager : KeyedManager<EventAttendeeMetrics>, IEventAttendeeMetricsManager
    {
        private const decimal KgToLbsConversion = 2.20462m;
        private const int WeightUnitPounds = 1;
        private const int WeightUnitKilograms = 2;

        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IKeyedManager<Event> eventManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttendeeMetricsManager"/> class.
        /// </summary>
        public EventAttendeeMetricsManager(
            IKeyedRepository<EventAttendeeMetrics> repository,
            IEventAttendeeManager eventAttendeeManager,
            IKeyedManager<Event> eventManager)
            : base(repository)
        {
            this.eventAttendeeManager = eventAttendeeManager;
            this.eventManager = eventManager;
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> SubmitMetricsAsync(
            Guid eventId,
            Guid userId,
            EventAttendeeMetrics metrics,
            CancellationToken cancellationToken = default)
        {
            // Validate event exists
            var eventEntity = await eventManager.GetAsync(eventId, cancellationToken).ConfigureAwait(false);
            if (eventEntity == null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Event not found.");
            }

            // Check if user is a registered attendee for this event
            var attendingEvents = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, false, cancellationToken)
                .ConfigureAwait(false);

            if (!attendingEvents.Any(e => e.Id == eventId))
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("You must be registered as an attendee for this event to submit metrics.");
            }

            // Check for existing submission
            var existingMetrics = await GetMyMetricsAsync(eventId, userId, cancellationToken).ConfigureAwait(false);

            if (existingMetrics != null)
            {
                // Update existing submission if still pending
                if (existingMetrics.Status != "Pending")
                {
                    return ServiceResult<EventAttendeeMetrics>.Failure("Your metrics have already been reviewed and cannot be modified.");
                }

                existingMetrics.BagsCollected = metrics.BagsCollected;
                existingMetrics.PickedWeight = metrics.PickedWeight;
                existingMetrics.PickedWeightUnitId = metrics.PickedWeightUnitId;
                existingMetrics.DurationMinutes = metrics.DurationMinutes;
                existingMetrics.Notes = metrics.Notes;
                existingMetrics.LastUpdatedByUserId = userId;
                existingMetrics.LastUpdatedDate = DateTimeOffset.UtcNow;

                var updatedResult = await base.UpdateAsync(existingMetrics, userId, cancellationToken).ConfigureAwait(false);
                return ServiceResult<EventAttendeeMetrics>.Success(updatedResult);
            }

            // Create new submission
            var newMetrics = new EventAttendeeMetrics
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                BagsCollected = metrics.BagsCollected,
                PickedWeight = metrics.PickedWeight,
                PickedWeightUnitId = metrics.PickedWeightUnitId,
                DurationMinutes = metrics.DurationMinutes,
                Notes = metrics.Notes,
                Status = "Pending",
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            var result = await base.AddAsync(newMetrics, userId, cancellationToken).ConfigureAwait(false);
            return ServiceResult<EventAttendeeMetrics>.Success(result);
        }

        /// <inheritdoc />
        public async Task<EventAttendeeMetrics> GetMyMetricsAsync(
            Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(m => m.PickedWeightUnit)
                .Include(m => m.AdjustedPickedWeightUnit)
                .FirstOrDefaultAsync(m => m.EventId == eventId && m.UserId == userId, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventAttendeeMetrics>> GetByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(m => m.EventId == eventId)
                .Include(m => m.User)
                .Include(m => m.PickedWeightUnit)
                .Include(m => m.AdjustedPickedWeightUnit)
                .OrderBy(m => m.User.UserName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventAttendeeMetrics>> GetPendingByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(m => m.EventId == eventId && m.Status == "Pending")
                .Include(m => m.User)
                .Include(m => m.PickedWeightUnit)
                .OrderBy(m => m.CreatedDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> ApproveAsync(
            Guid metricsId,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var metrics = await Repo.GetAsync(metricsId, cancellationToken).ConfigureAwait(false);
            if (metrics == null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Metrics submission not found.");
            }

            if (metrics.Status != "Pending")
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Only pending submissions can be approved.");
            }

            metrics.Status = "Approved";
            metrics.ReviewedByUserId = reviewerId;
            metrics.ReviewedDate = DateTimeOffset.UtcNow;
            metrics.LastUpdatedByUserId = reviewerId;
            metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(metrics, reviewerId, cancellationToken).ConfigureAwait(false);
            return ServiceResult<EventAttendeeMetrics>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> RejectAsync(
            Guid metricsId,
            string reason,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var metrics = await Repo.GetAsync(metricsId, cancellationToken).ConfigureAwait(false);
            if (metrics == null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Metrics submission not found.");
            }

            if (metrics.Status != "Pending")
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Only pending submissions can be rejected.");
            }

            metrics.Status = "Rejected";
            metrics.ReviewedByUserId = reviewerId;
            metrics.ReviewedDate = DateTimeOffset.UtcNow;
            metrics.RejectionReason = reason;
            metrics.LastUpdatedByUserId = reviewerId;
            metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(metrics, reviewerId, cancellationToken).ConfigureAwait(false);
            return ServiceResult<EventAttendeeMetrics>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> AdjustAsync(
            Guid metricsId,
            EventAttendeeMetrics adjustedValues,
            string reason,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var metrics = await Repo.GetAsync(metricsId, cancellationToken).ConfigureAwait(false);
            if (metrics == null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Metrics submission not found.");
            }

            if (metrics.Status != "Pending")
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Only pending submissions can be adjusted.");
            }

            metrics.Status = "Adjusted";
            metrics.ReviewedByUserId = reviewerId;
            metrics.ReviewedDate = DateTimeOffset.UtcNow;
            metrics.AdjustedBagsCollected = adjustedValues.AdjustedBagsCollected ?? adjustedValues.BagsCollected;
            metrics.AdjustedPickedWeight = adjustedValues.AdjustedPickedWeight ?? adjustedValues.PickedWeight;
            metrics.AdjustedPickedWeightUnitId = adjustedValues.AdjustedPickedWeightUnitId ?? adjustedValues.PickedWeightUnitId;
            metrics.AdjustedDurationMinutes = adjustedValues.AdjustedDurationMinutes ?? adjustedValues.DurationMinutes;
            metrics.AdjustmentReason = reason;
            metrics.LastUpdatedByUserId = reviewerId;
            metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(metrics, reviewerId, cancellationToken).ConfigureAwait(false);
            return ServiceResult<EventAttendeeMetrics>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<int>> ApproveAllPendingAsync(
            Guid eventId,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var pendingMetrics = await Repo.Get()
                .Where(m => m.EventId == eventId && m.Status == "Pending")
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (pendingMetrics.Count == 0)
            {
                return ServiceResult<int>.Success(0);
            }

            foreach (var metrics in pendingMetrics)
            {
                metrics.Status = "Approved";
                metrics.ReviewedByUserId = reviewerId;
                metrics.ReviewedDate = DateTimeOffset.UtcNow;
                metrics.LastUpdatedByUserId = reviewerId;
                metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

                await base.UpdateAsync(metrics, reviewerId, cancellationToken).ConfigureAwait(false);
            }

            return ServiceResult<int>.Success(pendingMetrics.Count);
        }

        /// <inheritdoc />
        public async Task<AttendeeMetricsTotals> CalculateTotalsAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var allMetrics = await Repo.Get()
                .Where(m => m.EventId == eventId)
                .Include(m => m.PickedWeightUnit)
                .Include(m => m.AdjustedPickedWeightUnit)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var approvedMetrics = allMetrics.Where(m => m.Status == "Approved" || m.Status == "Adjusted").ToList();

            var totals = new AttendeeMetricsTotals
            {
                TotalSubmissions = allMetrics.Count,
                ApprovedSubmissions = approvedMetrics.Count,
                PendingSubmissions = allMetrics.Count(m => m.Status == "Pending"),
                TotalBagsCollected = 0,
                TotalWeightPounds = 0,
                TotalDurationMinutes = 0
            };

            foreach (var metrics in approvedMetrics)
            {
                // Use adjusted values if available (for "Adjusted" status), otherwise original values
                int? bags;
                decimal? weight;
                int? weightUnitId;
                int? duration;

                if (metrics.Status == "Adjusted")
                {
                    bags = metrics.AdjustedBagsCollected ?? metrics.BagsCollected;
                    weight = metrics.AdjustedPickedWeight ?? metrics.PickedWeight;
                    weightUnitId = metrics.AdjustedPickedWeightUnitId ?? metrics.PickedWeightUnitId;
                    duration = metrics.AdjustedDurationMinutes ?? metrics.DurationMinutes;
                }
                else
                {
                    bags = metrics.BagsCollected;
                    weight = metrics.PickedWeight;
                    weightUnitId = metrics.PickedWeightUnitId;
                    duration = metrics.DurationMinutes;
                }

                if (bags.HasValue)
                {
                    totals.TotalBagsCollected += bags.Value;
                }

                if (weight.HasValue && weightUnitId.HasValue)
                {
                    // Convert to pounds for consistent aggregation
                    var weightInPounds = weightUnitId.Value == WeightUnitKilograms
                        ? weight.Value * KgToLbsConversion
                        : weight.Value;
                    totals.TotalWeightPounds += weightInPounds;
                }

                if (duration.HasValue)
                {
                    totals.TotalDurationMinutes += duration.Value;
                }
            }

            return totals;
        }

        /// <inheritdoc />
        public async Task<bool> HasSubmittedMetricsAsync(
            Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .AnyAsync(m => m.EventId == eventId && m.UserId == userId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
