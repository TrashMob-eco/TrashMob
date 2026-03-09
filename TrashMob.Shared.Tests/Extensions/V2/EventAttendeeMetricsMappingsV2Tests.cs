namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class EventAttendeeMetricsMappingsV2Tests
    {
        [Fact]
        public void EventAttendeeMetrics_ToV2Dto_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewedDate = DateTimeOffset.UtcNow;

            var entity = new EventAttendeeMetrics
            {
                Id = id,
                EventId = eventId,
                UserId = userId,
                BagsCollected = 5,
                PickedWeight = 12.5m,
                PickedWeightUnitId = 1,
                DurationMinutes = 120,
                Notes = "Great event",
                Status = "Approved",
                ReviewedDate = reviewedDate,
                RejectionReason = "",
                AdjustedBagsCollected = 6,
                AdjustedPickedWeight = 13.0m,
                AdjustedPickedWeightUnitId = 1,
                AdjustedDurationMinutes = 125,
                AdjustmentReason = "Corrected count",
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(id, dto.Id);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(5, dto.BagsCollected);
            Assert.Equal(12.5m, dto.PickedWeight);
            Assert.Equal(1, dto.PickedWeightUnitId);
            Assert.Equal(120, dto.DurationMinutes);
            Assert.Equal("Great event", dto.Notes);
            Assert.Equal("Approved", dto.Status);
            Assert.Equal(reviewedDate, dto.ReviewedDate);
            Assert.Equal("", dto.RejectionReason);
            Assert.Equal(6, dto.AdjustedBagsCollected);
            Assert.Equal(13.0m, dto.AdjustedPickedWeight);
            Assert.Equal(1, dto.AdjustedPickedWeightUnitId);
            Assert.Equal(125, dto.AdjustedDurationMinutes);
            Assert.Equal("Corrected count", dto.AdjustmentReason);
        }

        [Fact]
        public void EventAttendeeMetrics_ToV2Dto_HandlesNullStrings()
        {
            var entity = new EventAttendeeMetrics
            {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Notes = null,
                Status = null,
                RejectionReason = null,
                AdjustmentReason = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.Notes);
            Assert.Equal(string.Empty, dto.Status);
            Assert.Equal(string.Empty, dto.RejectionReason);
            Assert.Equal(string.Empty, dto.AdjustmentReason);
        }

        [Fact]
        public void EventAttendeeMetrics_ToV2Dto_HandlesNullNumerics()
        {
            var entity = new EventAttendeeMetrics
            {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BagsCollected = null,
                PickedWeight = null,
                PickedWeightUnitId = null,
                DurationMinutes = null,
                ReviewedDate = null,
                AdjustedBagsCollected = null,
                AdjustedPickedWeight = null,
                AdjustedPickedWeightUnitId = null,
                AdjustedDurationMinutes = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Null(dto.BagsCollected);
            Assert.Null(dto.PickedWeight);
            Assert.Null(dto.PickedWeightUnitId);
            Assert.Null(dto.DurationMinutes);
            Assert.Null(dto.ReviewedDate);
            Assert.Null(dto.AdjustedBagsCollected);
            Assert.Null(dto.AdjustedPickedWeight);
            Assert.Null(dto.AdjustedPickedWeightUnitId);
            Assert.Null(dto.AdjustedDurationMinutes);
        }
    }
}
