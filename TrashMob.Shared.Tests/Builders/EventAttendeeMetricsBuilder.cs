namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating EventAttendeeMetrics test data with sensible defaults.
    /// </summary>
    public class EventAttendeeMetricsBuilder
    {
        private readonly EventAttendeeMetrics _metrics;

        public EventAttendeeMetricsBuilder()
        {
            var userId = Guid.NewGuid();
            _metrics = new EventAttendeeMetrics
            {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                UserId = userId,
                BagsCollected = 5,
                PickedWeight = 10.5m,
                PickedWeightUnitId = 1, // Pounds
                DurationMinutes = 120,
                Notes = "Test notes",
                Status = "Pending",
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public EventAttendeeMetricsBuilder WithId(Guid id)
        {
            _metrics.Id = id;
            return this;
        }

        public EventAttendeeMetricsBuilder ForEvent(Guid eventId)
        {
            _metrics.EventId = eventId;
            return this;
        }

        public EventAttendeeMetricsBuilder ForUser(Guid userId)
        {
            _metrics.UserId = userId;
            _metrics.CreatedByUserId = userId;
            _metrics.LastUpdatedByUserId = userId;
            return this;
        }

        public EventAttendeeMetricsBuilder WithBagsCollected(int bags)
        {
            _metrics.BagsCollected = bags;
            return this;
        }

        public EventAttendeeMetricsBuilder WithWeight(decimal weight, int weightUnitId = 1)
        {
            _metrics.PickedWeight = weight;
            _metrics.PickedWeightUnitId = weightUnitId;
            return this;
        }

        public EventAttendeeMetricsBuilder WithWeightInPounds(decimal pounds)
        {
            _metrics.PickedWeight = pounds;
            _metrics.PickedWeightUnitId = 1; // Pounds
            return this;
        }

        public EventAttendeeMetricsBuilder WithWeightInKilograms(decimal kilograms)
        {
            _metrics.PickedWeight = kilograms;
            _metrics.PickedWeightUnitId = 2; // Kilograms
            return this;
        }

        public EventAttendeeMetricsBuilder WithDuration(int minutes)
        {
            _metrics.DurationMinutes = minutes;
            return this;
        }

        public EventAttendeeMetricsBuilder WithNotes(string notes)
        {
            _metrics.Notes = notes;
            return this;
        }

        public EventAttendeeMetricsBuilder AsPending()
        {
            _metrics.Status = "Pending";
            _metrics.ReviewedByUserId = null;
            _metrics.ReviewedDate = null;
            return this;
        }

        public EventAttendeeMetricsBuilder AsApproved(Guid? reviewerId = null)
        {
            _metrics.Status = "Approved";
            _metrics.ReviewedByUserId = reviewerId ?? Guid.NewGuid();
            _metrics.ReviewedDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EventAttendeeMetricsBuilder AsRejected(string reason, Guid? reviewerId = null)
        {
            _metrics.Status = "Rejected";
            _metrics.ReviewedByUserId = reviewerId ?? Guid.NewGuid();
            _metrics.ReviewedDate = DateTimeOffset.UtcNow;
            _metrics.RejectionReason = reason;
            return this;
        }

        public EventAttendeeMetricsBuilder AsAdjusted(int? bags = null, decimal? weight = null, int? duration = null, string reason = null, Guid? reviewerId = null)
        {
            _metrics.Status = "Adjusted";
            _metrics.ReviewedByUserId = reviewerId ?? Guid.NewGuid();
            _metrics.ReviewedDate = DateTimeOffset.UtcNow;
            _metrics.AdjustedBagsCollected = bags ?? _metrics.BagsCollected;
            _metrics.AdjustedPickedWeight = weight ?? _metrics.PickedWeight;
            _metrics.AdjustedPickedWeightUnitId = _metrics.PickedWeightUnitId;
            _metrics.AdjustedDurationMinutes = duration ?? _metrics.DurationMinutes;
            _metrics.AdjustmentReason = reason ?? "Adjusted by reviewer";
            return this;
        }

        public EventAttendeeMetricsBuilder WithUser(User user)
        {
            _metrics.UserId = user.Id;
            _metrics.User = user;
            _metrics.CreatedByUserId = user.Id;
            _metrics.LastUpdatedByUserId = user.Id;
            return this;
        }

        public EventAttendeeMetricsBuilder WithEvent(Event @event)
        {
            _metrics.EventId = @event.Id;
            _metrics.Event = @event;
            return this;
        }

        public EventAttendeeMetrics Build() => _metrics;
    }
}
