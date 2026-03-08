namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using System.Collections.Generic;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class LitterReportMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var imageId = Guid.NewGuid();
            var entity = new LitterReport
            {
                Id = Guid.NewGuid(),
                Name = "Park Litter",
                Description = "Litter near the fountain",
                LitterReportStatusId = 1,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
                LitterImages =
                [
                    new LitterImage
                    {
                        Id = imageId,
                        AzureBlobURL = "https://blob.example.com/img1.jpg",
                        StreetAddress = "123 Main St",
                        City = "Seattle",
                        Region = "WA",
                        Country = "US",
                        PostalCode = "98101",
                        Latitude = 47.6062,
                        Longitude = -122.3321,
                    },
                ],
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.LitterReportStatusId, dto.LitterReportStatusId);
            Assert.Equal(entity.CreatedByUserId, dto.CreatedByUserId);
            Assert.Equal(entity.CreatedDate, dto.CreatedDate);
            Assert.Equal(entity.LastUpdatedDate, dto.LastUpdatedDate);
            Assert.Single(dto.Images);
            Assert.Equal(imageId, dto.Images[0].Id);
            Assert.Equal("https://blob.example.com/img1.jpg", dto.Images[0].ImageUrl);
            Assert.Equal("Seattle", dto.Images[0].City);
        }

        [Fact]
        public void ToV2Dto_ExcludesCancelledImages()
        {
            var entity = new LitterReport
            {
                Id = Guid.NewGuid(),
                LitterImages = new List<LitterImage>
                {
                    new() { Id = Guid.NewGuid(), AzureBlobURL = "img1.jpg", IsCancelled = false },
                    new() { Id = Guid.NewGuid(), AzureBlobURL = "img2.jpg", IsCancelled = true },
                    new() { Id = Guid.NewGuid(), AzureBlobURL = "img3.jpg", IsCancelled = false },
                },
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(2, dto.Images.Count);
        }

        [Fact]
        public void ToV2Dto_HandlesNullImages()
        {
            var entity = new LitterReport
            {
                Id = Guid.NewGuid(),
                LitterImages = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Empty(dto.Images);
        }

        [Fact]
        public void ToV2Dto_HandlesNullDates()
        {
            var entity = new LitterReport
            {
                CreatedDate = null,
                LastUpdatedDate = null,
                LitterImages = [],
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(default, dto.CreatedDate);
            Assert.Equal(default, dto.LastUpdatedDate);
        }

        [Fact]
        public void LitterImage_ToV2Dto_MapsAllProperties()
        {
            var entity = new LitterImage
            {
                Id = Guid.NewGuid(),
                AzureBlobURL = "https://blob.example.com/photo.jpg",
                StreetAddress = "456 Oak Ave",
                City = "Portland",
                Region = "OR",
                Country = "US",
                PostalCode = "97201",
                Latitude = 45.5152,
                Longitude = -122.6784,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.AzureBlobURL, dto.ImageUrl);
            Assert.Equal(entity.StreetAddress, dto.StreetAddress);
            Assert.Equal(entity.City, dto.City);
            Assert.Equal(entity.Region, dto.Region);
            Assert.Equal(entity.Country, dto.Country);
            Assert.Equal(entity.PostalCode, dto.PostalCode);
            Assert.Equal(entity.Latitude, dto.Latitude);
            Assert.Equal(entity.Longitude, dto.Longitude);
        }

        [Fact]
        public void LitterImage_ToV2Dto_DoesNotExposeModerationFields()
        {
            var entity = new LitterImage
            {
                Id = Guid.NewGuid(),
                ModerationStatus = PhotoModerationStatus.Rejected,
                InReview = true,
                ReviewRequestedByUserId = Guid.NewGuid(),
                ModeratedByUserId = Guid.NewGuid(),
                ModerationReason = "Inappropriate",
            };

            var dto = entity.ToV2Dto();

            var dtoType = dto.GetType();
            Assert.Null(dtoType.GetProperty("ModerationStatus"));
            Assert.Null(dtoType.GetProperty("InReview"));
            Assert.Null(dtoType.GetProperty("ReviewRequestedByUserId"));
            Assert.Null(dtoType.GetProperty("ModeratedByUserId"));
            Assert.Null(dtoType.GetProperty("ModerationReason"));
            Assert.Null(dtoType.GetProperty("IsCancelled"));
        }
    }
}
