namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class PartnerMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var entity = new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Seattle Parks",
                Website = "https://seattle.gov/parks",
                PublicNotes = "Great partner",
                LogoUrl = "https://example.com/logo.png",
                PartnerStatusId = 1,
                PartnerTypeId = 3,
                Slug = "seattle-wa",
                HomePageEnabled = true,
                IsFeatured = true,
                BrandingPrimaryColor = "#3B82F6",
                BrandingSecondaryColor = "#1E40AF",
                BannerImageUrl = "https://example.com/banner.jpg",
                Tagline = "Keep Seattle Clean",
                Latitude = 47.6062,
                Longitude = -122.3321,
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PhysicalAddress = "123 Main St",
                BoundsNorth = 47.7,
                BoundsSouth = 47.5,
                BoundsEast = -122.2,
                BoundsWest = -122.4,
                BoundaryGeoJson = "{\"type\":\"Polygon\"}",
                RegionType = 1,
                CountyName = "King County",
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 2, 15, 0, 0, 0, TimeSpan.Zero),
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Website, dto.Website);
            Assert.Equal(entity.PublicNotes, dto.PublicNotes);
            Assert.Equal(entity.LogoUrl, dto.LogoUrl);
            Assert.Equal(entity.PartnerStatusId, dto.PartnerStatusId);
            Assert.Equal(entity.PartnerTypeId, dto.PartnerTypeId);
            Assert.Equal(entity.Slug, dto.Slug);
            Assert.Equal(entity.HomePageEnabled, dto.HomePageEnabled);
            Assert.Equal(entity.IsFeatured, dto.IsFeatured);
            Assert.Equal(entity.BrandingPrimaryColor, dto.BrandingPrimaryColor);
            Assert.Equal(entity.BrandingSecondaryColor, dto.BrandingSecondaryColor);
            Assert.Equal(entity.BannerImageUrl, dto.BannerImageUrl);
            Assert.Equal(entity.Tagline, dto.Tagline);
            Assert.Equal(entity.Latitude, dto.Latitude);
            Assert.Equal(entity.Longitude, dto.Longitude);
            Assert.Equal(entity.City, dto.City);
            Assert.Equal(entity.Region, dto.Region);
            Assert.Equal(entity.Country, dto.Country);
            Assert.Equal(entity.PhysicalAddress, dto.PhysicalAddress);
            Assert.Equal(entity.BoundsNorth, dto.BoundsNorth);
            Assert.Equal(entity.BoundsSouth, dto.BoundsSouth);
            Assert.Equal(entity.BoundsEast, dto.BoundsEast);
            Assert.Equal(entity.BoundsWest, dto.BoundsWest);
            Assert.Equal(entity.BoundaryGeoJson, dto.BoundaryGeoJson);
            Assert.Equal(entity.RegionType, dto.RegionType);
            Assert.Equal(entity.CountyName, dto.CountyName);
            Assert.Equal(entity.CreatedByUserId, dto.CreatedByUserId);
            Assert.Equal(entity.CreatedDate, dto.CreatedDate);
            Assert.Equal(entity.LastUpdatedDate, dto.LastUpdatedDate);
        }

        [Fact]
        public void ToV2Dto_HandlesNullDates()
        {
            var entity = new Partner
            {
                CreatedDate = null,
                LastUpdatedDate = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(default, dto.CreatedDate);
            Assert.Equal(default, dto.LastUpdatedDate);
        }

        [Fact]
        public void ToV2Dto_DoesNotExposeAdminFields()
        {
            var dto = new Partner().ToV2Dto();

            var dtoType = dto.GetType();
            Assert.Null(dtoType.GetProperty("PrivateNotes"));
            Assert.Null(dtoType.GetProperty("ContactEmail"));
            Assert.Null(dtoType.GetProperty("ContactPhone"));
            Assert.Null(dtoType.GetProperty("DefaultCleanupFrequencyDays"));
            Assert.Null(dtoType.GetProperty("DefaultMinEventsPerYear"));
            Assert.Null(dtoType.GetProperty("DefaultSafetyRequirements"));
            Assert.Null(dtoType.GetProperty("DefaultAllowCoAdoption"));
            Assert.Null(dtoType.GetProperty("HomePageStartDate"));
            Assert.Null(dtoType.GetProperty("HomePageEndDate"));
        }
    }
}
