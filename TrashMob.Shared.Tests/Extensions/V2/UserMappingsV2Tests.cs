namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class UserMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var entity = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                PrefersMetric = true,
                GivenName = "Test",
                Surname = "User",
                ProfilePhotoUrl = "https://example.com/photo.jpg",
                MemberSince = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                IsMinor = false,
                Email = "test@example.com",
                IsSiteAdmin = true,
                DateAgreedToTrashMobWaiver = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
                TrashMobWaiverVersion = "1.0",
                DateOfBirth = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.UserName, dto.UserName);
            Assert.Equal(entity.City, dto.City);
            Assert.Equal(entity.Region, dto.Region);
            Assert.Equal(entity.Country, dto.Country);
            Assert.Equal(entity.PostalCode, dto.PostalCode);
            Assert.Equal(entity.Latitude, dto.Latitude);
            Assert.Equal(entity.Longitude, dto.Longitude);
            Assert.Equal(entity.PrefersMetric, dto.PrefersMetric);
            Assert.Equal(entity.GivenName, dto.GivenName);
            Assert.Equal(entity.Surname, dto.Surname);
            Assert.Equal(entity.ProfilePhotoUrl, dto.ProfilePhotoUrl);
            Assert.Equal(entity.MemberSince, dto.MemberSince);
            Assert.Equal(entity.IsMinor, dto.IsMinor);
            Assert.Equal(entity.Email, dto.Email);
            Assert.Equal(entity.IsSiteAdmin, dto.IsSiteAdmin);
            Assert.Equal(entity.DateAgreedToTrashMobWaiver, dto.DateAgreedToTrashMobWaiver);
            Assert.Equal(entity.TrashMobWaiverVersion, dto.TrashMobWaiverVersion);
            Assert.Equal(entity.DateOfBirth, dto.DateOfBirth);
        }

        [Fact]
        public void ToV2Dto_HandlesNullMemberSince()
        {
            var entity = new User
            {
                MemberSince = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Null(dto.MemberSince);
        }

        [Fact]
        public void ToV2Dto_MapsAuthAndProfileFields()
        {
            var entity = new User
            {
                Email = "test@example.com",
                ObjectId = Guid.NewGuid(),
                NameIdentifier = "secret-identifier",
                DateOfBirth = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                IsSiteAdmin = true,
                DateAgreedToTrashMobWaiver = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
                TrashMobWaiverVersion = "1.0",
            };

            var dto = entity.ToV2Dto();

            Assert.Equal("test@example.com", dto.Email);
            Assert.True(dto.IsSiteAdmin);
            Assert.Equal(entity.DateOfBirth, dto.DateOfBirth);
            Assert.Equal(entity.DateAgreedToTrashMobWaiver, dto.DateAgreedToTrashMobWaiver);
            Assert.Equal("1.0", dto.TrashMobWaiverVersion);

            // Identity provider fields are still excluded from UserDto
            var dtoType = dto.GetType();
            Assert.Null(dtoType.GetProperty("ObjectId"));
            Assert.Null(dtoType.GetProperty("NameIdentifier"));
        }

        [Fact]
        public void ToV2Dto_MapsTravelLimitForLocalEvents()
        {
            var entity = new User
            {
                TravelLimitForLocalEvents = 25,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(25, dto.TravelLimitForLocalEvents);
        }

        [Fact]
        public void UserDto_ToEntity_MapsAllProperties()
        {
            var dto = new Models.Poco.V2.UserDto
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                PrefersMetric = true,
                GivenName = "Test",
                Surname = "User",
                ProfilePhotoUrl = "https://example.com/photo.jpg",
                MemberSince = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                IsMinor = false,
                TravelLimitForLocalEvents = 15,
                Email = "test@example.com",
                IsSiteAdmin = true,
                DateAgreedToTrashMobWaiver = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
                TrashMobWaiverVersion = "1.0",
                DateOfBirth = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
            };

            var entity = dto.ToEntity();

            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.UserName, entity.UserName);
            Assert.Equal(dto.City, entity.City);
            Assert.Equal(dto.Region, entity.Region);
            Assert.Equal(dto.Country, entity.Country);
            Assert.Equal(dto.PostalCode, entity.PostalCode);
            Assert.Equal(dto.Latitude, entity.Latitude);
            Assert.Equal(dto.Longitude, entity.Longitude);
            Assert.Equal(dto.PrefersMetric, entity.PrefersMetric);
            Assert.Equal(dto.GivenName, entity.GivenName);
            Assert.Equal(dto.Surname, entity.Surname);
            Assert.Equal(dto.ProfilePhotoUrl, entity.ProfilePhotoUrl);
            Assert.Equal(dto.MemberSince, entity.MemberSince);
            Assert.Equal(dto.IsMinor, entity.IsMinor);
            Assert.Equal(dto.TravelLimitForLocalEvents, entity.TravelLimitForLocalEvents);
            Assert.Equal(dto.Email, entity.Email);
            Assert.Equal(dto.IsSiteAdmin, entity.IsSiteAdmin);
            Assert.Equal(dto.DateAgreedToTrashMobWaiver, entity.DateAgreedToTrashMobWaiver);
            Assert.Equal(dto.TrashMobWaiverVersion, entity.TrashMobWaiverVersion);
            Assert.Equal(dto.DateOfBirth, entity.DateOfBirth);
        }

        [Fact]
        public void UserWriteDto_ToEntity_MapsAllProperties()
        {
            var dto = new Models.Poco.V2.UserWriteDto
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                GivenName = "Test",
                Surname = "User",
                City = "Portland",
                Region = "OR",
                Country = "US",
                PostalCode = "97201",
                Latitude = 45.5152,
                Longitude = -122.6784,
                PrefersMetric = false,
                DateOfBirth = new DateTimeOffset(1990, 6, 15, 0, 0, 0, TimeSpan.Zero),
                IsMinor = false,
                TravelLimitForLocalEvents = 30,
                ProfilePhotoUrl = "https://example.com/photo2.jpg",
            };

            var entity = dto.ToEntity();

            Assert.Equal(dto.Id, entity.Id);
            Assert.Equal(dto.UserName, entity.UserName);
            Assert.Equal(dto.Email, entity.Email);
            Assert.Equal(dto.GivenName, entity.GivenName);
            Assert.Equal(dto.Surname, entity.Surname);
            Assert.Equal(dto.City, entity.City);
            Assert.Equal(dto.Region, entity.Region);
            Assert.Equal(dto.Country, entity.Country);
            Assert.Equal(dto.PostalCode, entity.PostalCode);
            Assert.Equal(dto.Latitude, entity.Latitude);
            Assert.Equal(dto.Longitude, entity.Longitude);
            Assert.Equal(dto.PrefersMetric, entity.PrefersMetric);
            Assert.Equal(dto.DateOfBirth, entity.DateOfBirth);
            Assert.Equal(dto.IsMinor, entity.IsMinor);
            Assert.Equal(dto.TravelLimitForLocalEvents, entity.TravelLimitForLocalEvents);
            Assert.Equal(dto.ProfilePhotoUrl, entity.ProfilePhotoUrl);
        }

        [Fact]
        public void UserWriteDto_ToEntity_HandlesNullEmail()
        {
            var dto = new Models.Poco.V2.UserWriteDto
            {
                Id = Guid.NewGuid(),
                Email = null,
            };

            var entity = dto.ToEntity();

            Assert.Equal(string.Empty, entity.Email);
        }

        [Fact]
        public void ToWriteDto_MapsAllProperties()
        {
            var entity = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                GivenName = "Test",
                Surname = "User",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                PrefersMetric = true,
                DateOfBirth = new DateTimeOffset(1995, 3, 20, 0, 0, 0, TimeSpan.Zero),
                IsMinor = false,
                TravelLimitForLocalEvents = 20,
                ProfilePhotoUrl = "https://example.com/photo.jpg",
            };

            var dto = entity.ToWriteDto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.UserName, dto.UserName);
            Assert.Equal(entity.Email, dto.Email);
            Assert.Equal(entity.GivenName, dto.GivenName);
            Assert.Equal(entity.Surname, dto.Surname);
            Assert.Equal(entity.City, dto.City);
            Assert.Equal(entity.Region, dto.Region);
            Assert.Equal(entity.Country, dto.Country);
            Assert.Equal(entity.PostalCode, dto.PostalCode);
            Assert.Equal(entity.Latitude, dto.Latitude);
            Assert.Equal(entity.Longitude, dto.Longitude);
            Assert.Equal(entity.PrefersMetric, dto.PrefersMetric);
            Assert.Equal(entity.DateOfBirth, dto.DateOfBirth);
            Assert.Equal(entity.IsMinor, dto.IsMinor);
            Assert.Equal(entity.TravelLimitForLocalEvents, dto.TravelLimitForLocalEvents);
            Assert.Equal(entity.ProfilePhotoUrl, dto.ProfilePhotoUrl);
        }

        [Fact]
        public void RoundTrip_UserDtoToEntityAndBack_PreservesProperties()
        {
            var entity = new User
            {
                Id = Guid.NewGuid(),
                UserName = "roundtrip",
                City = "Issaquah",
                Region = "Washington",
                Country = "United States",
                PostalCode = "98027",
                Latitude = 47.5301,
                Longitude = -122.0326,
                PrefersMetric = false,
                GivenName = "Round",
                Surname = "Trip",
                ProfilePhotoUrl = "https://example.com/rt.jpg",
                MemberSince = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
                IsMinor = false,
                TravelLimitForLocalEvents = 10,
                Email = "roundtrip@example.com",
                IsSiteAdmin = true,
                DateAgreedToTrashMobWaiver = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
                TrashMobWaiverVersion = "2.0",
                DateOfBirth = new DateTimeOffset(1985, 5, 10, 0, 0, 0, TimeSpan.Zero),
            };

            var roundTripped = entity.ToV2Dto().ToEntity();

            Assert.Equal(entity.Id, roundTripped.Id);
            Assert.Equal(entity.UserName, roundTripped.UserName);
            Assert.Equal(entity.City, roundTripped.City);
            Assert.Equal(entity.Region, roundTripped.Region);
            Assert.Equal(entity.Country, roundTripped.Country);
            Assert.Equal(entity.PostalCode, roundTripped.PostalCode);
            Assert.Equal(entity.Latitude, roundTripped.Latitude);
            Assert.Equal(entity.Longitude, roundTripped.Longitude);
            Assert.Equal(entity.PrefersMetric, roundTripped.PrefersMetric);
            Assert.Equal(entity.GivenName, roundTripped.GivenName);
            Assert.Equal(entity.Surname, roundTripped.Surname);
            Assert.Equal(entity.ProfilePhotoUrl, roundTripped.ProfilePhotoUrl);
            Assert.Equal(entity.MemberSince, roundTripped.MemberSince);
            Assert.Equal(entity.IsMinor, roundTripped.IsMinor);
            Assert.Equal(entity.TravelLimitForLocalEvents, roundTripped.TravelLimitForLocalEvents);
            Assert.Equal(entity.Email, roundTripped.Email);
            Assert.Equal(entity.IsSiteAdmin, roundTripped.IsSiteAdmin);
            Assert.Equal(entity.DateAgreedToTrashMobWaiver, roundTripped.DateAgreedToTrashMobWaiver);
            Assert.Equal(entity.TrashMobWaiverVersion, roundTripped.TrashMobWaiverVersion);
            Assert.Equal(entity.DateOfBirth, roundTripped.DateOfBirth);
        }
    }
}
