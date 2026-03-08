namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class TeamMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var entity = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Seattle Cleanup Crew",
                Description = "Cleaning up Seattle parks",
                LogoUrl = "https://example.com/logo.png",
                IsPublic = true,
                RequiresApproval = false,
                Latitude = 47.6062,
                Longitude = -122.3321,
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PostalCode = "98101",
                IsActive = true,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2025, 6, 15, 0, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero),
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.LogoUrl, dto.LogoUrl);
            Assert.Equal(entity.IsPublic, dto.IsPublic);
            Assert.Equal(entity.RequiresApproval, dto.RequiresApproval);
            Assert.Equal(entity.Latitude, dto.Latitude);
            Assert.Equal(entity.Longitude, dto.Longitude);
            Assert.Equal(entity.City, dto.City);
            Assert.Equal(entity.Region, dto.Region);
            Assert.Equal(entity.Country, dto.Country);
            Assert.Equal(entity.PostalCode, dto.PostalCode);
            Assert.Equal(entity.IsActive, dto.IsActive);
            Assert.Equal(entity.CreatedByUserId, dto.CreatedByUserId);
            Assert.Equal(entity.CreatedDate, dto.CreatedDate);
            Assert.Equal(entity.LastUpdatedDate, dto.LastUpdatedDate);
        }

        [Fact]
        public void ToV2Dto_HandlesNullDates()
        {
            var entity = new Team
            {
                CreatedDate = null,
                LastUpdatedDate = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(default, dto.CreatedDate);
            Assert.Equal(default, dto.LastUpdatedDate);
        }

        [Fact]
        public void ToV2Dto_DoesNotExposeNavigationProperties()
        {
            var dto = new Team().ToV2Dto();

            var dtoType = dto.GetType();
            Assert.Null(dtoType.GetProperty("Members"));
            Assert.Null(dtoType.GetProperty("JoinRequests"));
            Assert.Null(dtoType.GetProperty("TeamEvents"));
            Assert.Null(dtoType.GetProperty("Photos"));
            Assert.Null(dtoType.GetProperty("Adoptions"));
        }
    }
}
