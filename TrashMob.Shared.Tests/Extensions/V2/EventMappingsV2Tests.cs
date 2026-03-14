namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class EventMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var teamId = Guid.NewGuid();
            var lastUpdatedByUserId = Guid.NewGuid();
            var entity = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Park Cleanup",
                Description = "Clean up the local park",
                EventDate = new DateTimeOffset(2026, 6, 15, 10, 0, 0, TimeSpan.Zero),
                DurationHours = 2,
                DurationMinutes = 30,
                EventTypeId = 1,
                EventStatusId = (int)EventStatusEnum.Active,
                StreetAddress = "123 Main St",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                MaxNumberOfParticipants = 50,
                EventVisibilityId = (int)EventVisibilityEnum.Public,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastUpdatedByUserId = lastUpdatedByUserId,
                LastUpdatedDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
                TeamId = teamId,
                CreatedByUser = new User { UserName = "creator" },
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.EventDate, dto.EventDate);
            Assert.Equal(entity.DurationHours, dto.DurationHours);
            Assert.Equal(entity.DurationMinutes, dto.DurationMinutes);
            Assert.Equal(entity.EventTypeId, dto.EventTypeId);
            Assert.Equal(entity.EventStatusId, dto.EventStatusId);
            Assert.Equal(entity.StreetAddress, dto.StreetAddress);
            Assert.Equal(entity.City, dto.City);
            Assert.Equal(entity.Region, dto.Region);
            Assert.Equal(entity.Country, dto.Country);
            Assert.Equal(entity.PostalCode, dto.PostalCode);
            Assert.Equal(entity.Latitude, dto.Latitude);
            Assert.Equal(entity.Longitude, dto.Longitude);
            Assert.Equal(entity.MaxNumberOfParticipants, dto.MaxNumberOfParticipants);
            Assert.True(dto.IsEventPublic);
            Assert.Equal((int)EventVisibilityEnum.Public, dto.EventVisibilityId);
            Assert.Equal(entity.CreatedByUserId, dto.CreatedByUserId);
            Assert.Equal(entity.CreatedDate, dto.CreatedDate);
            Assert.Equal(lastUpdatedByUserId, dto.LastUpdatedByUserId);
            Assert.Equal(entity.LastUpdatedDate, dto.LastUpdatedDate);
            Assert.Equal(teamId, dto.TeamId);
            Assert.Equal("creator", dto.CreatedByUserName);
        }

        [Fact]
        public void ToV2Dto_IsEventPublic_TrueForPublicVisibility()
        {
            var entity = new Event { EventVisibilityId = (int)EventVisibilityEnum.Public };

            var dto = entity.ToV2Dto();

            Assert.True(dto.IsEventPublic);
        }

        [Fact]
        public void ToV2Dto_IsEventPublic_FalseForTeamOnlyVisibility()
        {
            var entity = new Event { EventVisibilityId = (int)EventVisibilityEnum.TeamOnly };

            var dto = entity.ToV2Dto();

            Assert.False(dto.IsEventPublic);
        }

        [Fact]
        public void ToV2Dto_IsEventPublic_FalseForPrivateVisibility()
        {
            var entity = new Event { EventVisibilityId = (int)EventVisibilityEnum.Private };

            var dto = entity.ToV2Dto();

            Assert.False(dto.IsEventPublic);
        }

        [Fact]
        public void ToV2Dto_HandlesNullDates()
        {
            var entity = new Event
            {
                CreatedDate = null,
                LastUpdatedDate = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(default, dto.CreatedDate);
            Assert.Equal(default, dto.LastUpdatedDate);
        }
    }
}
