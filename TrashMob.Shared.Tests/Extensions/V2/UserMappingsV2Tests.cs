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
        public void ToV2Dto_DoesNotExposePiiFields()
        {
            var entity = new User
            {
                Email = "secret@example.com",
                ObjectId = Guid.NewGuid(),
                NameIdentifier = "secret-identifier",
                DateOfBirth = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero),
                IsSiteAdmin = true,
            };

            var dto = entity.ToV2Dto();

            var dtoType = dto.GetType();
            Assert.Null(dtoType.GetProperty("Email"));
            Assert.Null(dtoType.GetProperty("ObjectId"));
            Assert.Null(dtoType.GetProperty("NameIdentifier"));
            Assert.Null(dtoType.GetProperty("DateOfBirth"));
            Assert.Null(dtoType.GetProperty("IsSiteAdmin"));
        }
    }
}
