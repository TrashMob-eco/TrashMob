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
    public class EventAttendeeMetricsManager(
        IKeyedRepository<EventAttendeeMetrics> repository,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<Event> eventManager)
        : KeyedManager<EventAttendeeMetrics>(repository), IEventAttendeeMetricsManager
    {
        private const decimal KgToLbsConversion = 2.20462m;
        private const int WeightUnitPounds = 1;
        private const int WeightUnitKilograms = 2;

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> SubmitMetricsAsync(
            Guid eventId,
            Guid userId,
            EventAttendeeMetrics metrics,
            CancellationToken cancellationToken = default)
        {
            // Validate event exists
            var eventEntity = await eventManager.GetAsync(eventId, cancellationToken);
            if (eventEntity is null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Event not found.");
            }

            // Check if user is a registered attendee for this event
            var attendingEvents = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, false, cancellationToken);

            if (!attendingEvents.Any(e => e.Id == eventId))
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("You must be registered as an attendee for this event to submit metrics.");
            }

            // Check for existing submission
            var existingMetrics = await GetMyMetricsAsync(eventId, userId, cancellationToken);

            if (existingMetrics is not null)
            {
                // Update existing submission if still pending
                if (existingMetrics.Status != EventAttendeeMetricsStatus.Pending)
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

                var updatedResult = await base.UpdateAsync(existingMetrics, userId, cancellationToken);
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
                Status = EventAttendeeMetricsStatus.Pending,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            var result = await base.AddAsync(newMetrics, userId, cancellationToken);
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
                .FirstOrDefaultAsync(m => m.EventId == eventId && m.UserId == userId, cancellationToken);
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
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventAttendeeMetrics>> GetPendingByEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(m => m.EventId == eventId && m.Status == EventAttendeeMetricsStatus.Pending)
                .Include(m => m.User)
                .Include(m => m.PickedWeightUnit)
                .OrderBy(m => m.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> ApproveAsync(
            Guid metricsId,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var metrics = await Repo.GetAsync(metricsId, cancellationToken);
            if (metrics is null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Metrics submission not found.");
            }

            if (metrics.Status != EventAttendeeMetricsStatus.Pending)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Only pending submissions can be approved.");
            }

            metrics.Status = EventAttendeeMetricsStatus.Approved;
            metrics.ReviewedByUserId = reviewerId;
            metrics.ReviewedDate = DateTimeOffset.UtcNow;
            metrics.LastUpdatedByUserId = reviewerId;
            metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(metrics, reviewerId, cancellationToken);
            return ServiceResult<EventAttendeeMetrics>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<EventAttendeeMetrics>> RejectAsync(
            Guid metricsId,
            string reason,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var metrics = await Repo.GetAsync(metricsId, cancellationToken);
            if (metrics is null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Metrics submission not found.");
            }

            if (metrics.Status != EventAttendeeMetricsStatus.Pending)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Only pending submissions can be rejected.");
            }

            metrics.Status = EventAttendeeMetricsStatus.Rejected;
            metrics.ReviewedByUserId = reviewerId;
            metrics.ReviewedDate = DateTimeOffset.UtcNow;
            metrics.RejectionReason = reason;
            metrics.LastUpdatedByUserId = reviewerId;
            metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(metrics, reviewerId, cancellationToken);
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
            var metrics = await Repo.GetAsync(metricsId, cancellationToken);
            if (metrics is null)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Metrics submission not found.");
            }

            if (metrics.Status != EventAttendeeMetricsStatus.Pending)
            {
                return ServiceResult<EventAttendeeMetrics>.Failure("Only pending submissions can be adjusted.");
            }

            metrics.Status = EventAttendeeMetricsStatus.Adjusted;
            metrics.ReviewedByUserId = reviewerId;
            metrics.ReviewedDate = DateTimeOffset.UtcNow;
            metrics.AdjustedBagsCollected = adjustedValues.AdjustedBagsCollected ?? adjustedValues.BagsCollected;
            metrics.AdjustedPickedWeight = adjustedValues.AdjustedPickedWeight ?? adjustedValues.PickedWeight;
            metrics.AdjustedPickedWeightUnitId = adjustedValues.AdjustedPickedWeightUnitId ?? adjustedValues.PickedWeightUnitId;
            metrics.AdjustedDurationMinutes = adjustedValues.AdjustedDurationMinutes ?? adjustedValues.DurationMinutes;
            metrics.AdjustmentReason = reason;
            metrics.LastUpdatedByUserId = reviewerId;
            metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await base.UpdateAsync(metrics, reviewerId, cancellationToken);
            return ServiceResult<EventAttendeeMetrics>.Success(result);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<int>> ApproveAllPendingAsync(
            Guid eventId,
            Guid reviewerId,
            CancellationToken cancellationToken = default)
        {
            var pendingMetrics = await Repo.Get()
                .Where(m => m.EventId == eventId && m.Status == EventAttendeeMetricsStatus.Pending)
                .ToListAsync(cancellationToken);

            if (pendingMetrics.Count == 0)
            {
                return ServiceResult<int>.Success(0);
            }

            foreach (var metrics in pendingMetrics)
            {
                metrics.Status = EventAttendeeMetricsStatus.Approved;
                metrics.ReviewedByUserId = reviewerId;
                metrics.ReviewedDate = DateTimeOffset.UtcNow;
                metrics.LastUpdatedByUserId = reviewerId;
                metrics.LastUpdatedDate = DateTimeOffset.UtcNow;

                await base.UpdateAsync(metrics, reviewerId, cancellationToken);
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
                .ToListAsync(cancellationToken);

            var approvedMetrics = allMetrics.Where(m => m.Status == EventAttendeeMetricsStatus.Approved || m.Status == EventAttendeeMetricsStatus.Adjusted).ToList();

            var totals = new AttendeeMetricsTotals
            {
                TotalSubmissions = allMetrics.Count,
                ApprovedSubmissions = approvedMetrics.Count,
                PendingSubmissions = allMetrics.Count(m => m.Status == EventAttendeeMetricsStatus.Pending),
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

                if (metrics.Status == EventAttendeeMetricsStatus.Adjusted)
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
                .AnyAsync(m => m.EventId == eventId && m.UserId == userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventMetricsPublicSummary> GetPublicMetricsSummaryAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var approvedMetrics = await Repo.Get()
                .Where(m => m.EventId == eventId && (m.Status == EventAttendeeMetricsStatus.Approved || m.Status == EventAttendeeMetricsStatus.Adjusted))
                .Include(m => m.User)
                .Include(m => m.PickedWeightUnit)
                .Include(m => m.AdjustedPickedWeightUnit)
                .ToListAsync(cancellationToken);

            var summary = new EventMetricsPublicSummary
            {
                EventId = eventId,
                ContributorCount = approvedMetrics.Count
            };

            foreach (var metrics in approvedMetrics)
            {
                int? bags;
                decimal? weight;
                int? weightUnitId;
                int? duration;

                if (metrics.Status == EventAttendeeMetricsStatus.Adjusted)
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

                // Calculate totals
                if (bags.HasValue)
                {
                    summary.TotalBagsCollected += bags.Value;
                }

                decimal weightInPounds = 0;
                if (weight.HasValue && weightUnitId.HasValue)
                {
                    weightInPounds = weightUnitId.Value == WeightUnitKilograms
                        ? weight.Value * KgToLbsConversion
                        : weight.Value;
                    summary.TotalWeightPounds += weightInPounds;
                }

                if (duration.HasValue)
                {
                    summary.TotalDurationMinutes += duration.Value;
                }

                // Add to contributors list (all approved metrics are public by default)
                if (metrics.User is not null)
                {
                    summary.Contributors.Add(new PublicAttendeeMetrics
                    {
                        UserId = metrics.UserId,
                        UserName = metrics.User.UserName,
                        BagsCollected = bags,
                        WeightPounds = weightInPounds > 0 ? weightInPounds : null,
                        DurationMinutes = duration,
                        Status = metrics.Status
                    });
                }
            }

            // Sort contributors by bags collected descending (leaderboard)
            summary.Contributors = summary.Contributors
                .OrderByDescending(c => c.BagsCollected ?? 0)
                .ThenByDescending(c => c.WeightPounds ?? 0)
                .ToList();

            return summary;
        }

        /// <inheritdoc />
        public async Task<UserImpactStats> GetUserImpactStatsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var userMetrics = await Repo.Get()
                .Where(m => m.UserId == userId && (m.Status == EventAttendeeMetricsStatus.Approved || m.Status == EventAttendeeMetricsStatus.Adjusted))
                .Include(m => m.Event)
                .Include(m => m.PickedWeightUnit)
                .Include(m => m.AdjustedPickedWeightUnit)
                .OrderByDescending(m => m.Event.EventDate)
                .ToListAsync(cancellationToken);

            var stats = new UserImpactStats
            {
                EventsWithMetrics = userMetrics.Count
            };

            foreach (var metrics in userMetrics)
            {
                int? bags;
                decimal? weight;
                int? weightUnitId;
                int? duration;

                if (metrics.Status == EventAttendeeMetricsStatus.Adjusted)
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

                decimal weightInPounds = 0;
                if (weight.HasValue && weightUnitId.HasValue)
                {
                    weightInPounds = weightUnitId.Value == WeightUnitKilograms
                        ? weight.Value * KgToLbsConversion
                        : weight.Value;
                }

                if (bags.HasValue)
                {
                    stats.TotalBagsCollected += bags.Value;
                }

                stats.TotalWeightPounds += weightInPounds;
                stats.TotalWeightKilograms += weightInPounds / KgToLbsConversion;

                if (duration.HasValue)
                {
                    stats.TotalDurationMinutes += duration.Value;
                }

                // Add event breakdown
                if (metrics.Event is not null)
                {
                    stats.EventBreakdown.Add(new UserEventMetricsSummary
                    {
                        EventId = metrics.EventId,
                        EventName = metrics.Event.Name,
                        EventDate = metrics.Event.EventDate,
                        BagsCollected = bags ?? 0,
                        WeightPounds = weightInPounds,
                        DurationMinutes = duration ?? 0,
                        Status = metrics.Status
                    });
                }
            }

            return stats;
        }
    }
}

