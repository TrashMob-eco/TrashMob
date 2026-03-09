namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using Xunit;

    public class PickupLocationMappingsV2Tests
    {
        [Fact]
        public void PickupLocation_ToV2Dto_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var entity = new PickupLocation
            {
                Id = id,
                EventId = eventId,
                Name = "Corner of 1st & Main",
                StreetAddress = "100 Main St",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98101",
                Country = "US",
                County = "King",
                Latitude = 47.6062,
                Longitude = -122.3321,
                HasBeenSubmitted = true,
                HasBeenPickedUp = false,
                Notes = "Near the park entrance",
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(id, dto.Id);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal("Corner of 1st & Main", dto.Name);
            Assert.Equal("100 Main St", dto.StreetAddress);
            Assert.Equal("Seattle", dto.City);
            Assert.Equal("WA", dto.Region);
            Assert.Equal("98101", dto.PostalCode);
            Assert.Equal("US", dto.Country);
            Assert.Equal("King", dto.County);
            Assert.Equal(47.6062, dto.Latitude);
            Assert.Equal(-122.3321, dto.Longitude);
            Assert.True(dto.HasBeenSubmitted);
            Assert.False(dto.HasBeenPickedUp);
            Assert.Equal("Near the park entrance", dto.Notes);
        }

        [Fact]
        public void PickupLocationDto_ToEntity_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var dto = new PickupLocationDto
            {
                Id = id,
                EventId = eventId,
                Name = "Test Location",
                StreetAddress = "456 Oak Ave",
                City = "Portland",
                Region = "OR",
                PostalCode = "97201",
                Country = "US",
                County = "Multnomah",
                Latitude = 45.5152,
                Longitude = -122.6784,
                HasBeenSubmitted = false,
                HasBeenPickedUp = true,
                Notes = "Test notes",
            };

            var entity = dto.ToEntity();

            Assert.Equal(id, entity.Id);
            Assert.Equal(eventId, entity.EventId);
            Assert.Equal("Test Location", entity.Name);
            Assert.Equal("456 Oak Ave", entity.StreetAddress);
            Assert.Equal("Portland", entity.City);
            Assert.Equal("OR", entity.Region);
            Assert.Equal("97201", entity.PostalCode);
            Assert.Equal("US", entity.Country);
            Assert.Equal("Multnomah", entity.County);
            Assert.Equal(45.5152, entity.Latitude);
            Assert.Equal(-122.6784, entity.Longitude);
            Assert.False(entity.HasBeenSubmitted);
            Assert.True(entity.HasBeenPickedUp);
            Assert.Equal("Test notes", entity.Notes);
        }

        [Fact]
        public void PickupLocation_ToV2Dto_HandlesNullStrings()
        {
            var entity = new PickupLocation
            {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                Name = null,
                StreetAddress = null,
                City = null,
                Region = null,
                PostalCode = null,
                Country = null,
                County = null,
                Notes = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.StreetAddress);
            Assert.Equal(string.Empty, dto.City);
            Assert.Equal(string.Empty, dto.Region);
            Assert.Equal(string.Empty, dto.PostalCode);
            Assert.Equal(string.Empty, dto.Country);
            Assert.Equal(string.Empty, dto.County);
            Assert.Equal(string.Empty, dto.Notes);
        }
    }
}
