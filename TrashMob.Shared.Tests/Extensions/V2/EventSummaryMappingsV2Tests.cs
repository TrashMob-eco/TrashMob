namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class EventSummaryMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var entity = new EventSummary
            {
                EventId = Guid.NewGuid(),
                NumberOfBuckets = 5,
                NumberOfBags = 12,
                DurationInMinutes = 120,
                ActualNumberOfAttendees = 8,
                PickedWeight = 45.5m,
                PickedWeightUnitId = 1,
                IsFromRouteData = true,
                Notes = "Great cleanup!",
                CreatedDate = new DateTimeOffset(2026, 3, 1, 10, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 3, 2, 14, 0, 0, TimeSpan.Zero),
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.EventId, dto.EventId);
            Assert.Equal(5, dto.NumberOfBuckets);
            Assert.Equal(12, dto.NumberOfBags);
            Assert.Equal(120, dto.DurationInMinutes);
            Assert.Equal(8, dto.ActualNumberOfAttendees);
            Assert.Equal(45.5m, dto.PickedWeight);
            Assert.Equal(1, dto.PickedWeightUnitId);
            Assert.True(dto.IsFromRouteData);
            Assert.Equal("Great cleanup!", dto.Notes);
            Assert.Equal(entity.CreatedDate, dto.CreatedDate);
            Assert.Equal(entity.LastUpdatedDate, dto.LastUpdatedDate);
        }

        [Fact]
        public void ToV2Dto_HandlesNullNotes()
        {
            var entity = new EventSummary
            {
                EventId = Guid.NewGuid(),
                Notes = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.Notes);
        }

        [Fact]
        public void ToV2Dto_HandlesNullAuditDates()
        {
            var entity = new EventSummary
            {
                EventId = Guid.NewGuid(),
                CreatedDate = null,
                LastUpdatedDate = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(default, dto.CreatedDate);
            Assert.Equal(default, dto.LastUpdatedDate);
        }
    }
}
