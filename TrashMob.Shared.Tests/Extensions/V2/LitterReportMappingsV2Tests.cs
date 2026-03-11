namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        [Fact]
        public void ToEntity_MapsAllProperties()
        {
            var reportId = Guid.NewGuid();
            var imageId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dto = new Models.Poco.V2.LitterReportDto
            {
                Id = reportId,
                Name = "Park Litter",
                Description = "Litter near the fountain",
                LitterReportStatusId = 2,
                CreatedByUserId = userId,
                CreatedDate = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero),
                Images =
                [
                    new Models.Poco.V2.LitterImageDto
                    {
                        Id = imageId,
                        ImageUrl = "https://blob.example.com/img1.jpg",
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

            var entity = dto.ToEntity();

            Assert.Equal(reportId, entity.Id);
            Assert.Equal("Park Litter", entity.Name);
            Assert.Equal("Litter near the fountain", entity.Description);
            Assert.Equal(2, entity.LitterReportStatusId);
            Assert.Equal(userId, entity.CreatedByUserId);
            Assert.NotNull(entity.LitterImages);
            Assert.Single(entity.LitterImages);
            var image = entity.LitterImages.First();
            Assert.Equal(imageId, image.Id);
            Assert.Equal(reportId, image.LitterReportId);
            Assert.Equal("https://blob.example.com/img1.jpg", image.AzureBlobURL);
            Assert.Equal("Seattle", image.City);
            Assert.Equal("WA", image.Region);
            Assert.Equal("US", image.Country);
            Assert.Equal("98101", image.PostalCode);
            Assert.Equal(47.6062, image.Latitude);
            Assert.Equal(-122.3321, image.Longitude);
        }

        [Fact]
        public void ToEntity_MapsEmptyImages()
        {
            var dto = new Models.Poco.V2.LitterReportDto
            {
                Id = Guid.NewGuid(),
                Images = [],
            };

            var entity = dto.ToEntity();

            Assert.NotNull(entity.LitterImages);
            Assert.Empty(entity.LitterImages);
        }

        [Fact]
        public void RoundTrip_EntityToDtoToEntity_PreservesProperties()
        {
            var imageId = Guid.NewGuid();
            var entity = new LitterReport
            {
                Id = Guid.NewGuid(),
                Name = "Beach Cleanup",
                Description = "Litter on the shore",
                LitterReportStatusId = 1,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 3, 5, 0, 0, 0, TimeSpan.Zero),
                LitterImages =
                [
                    new LitterImage
                    {
                        Id = imageId,
                        AzureBlobURL = "https://blob.example.com/beach.jpg",
                        StreetAddress = "1 Beach Rd",
                        City = "Malibu",
                        Region = "CA",
                        Country = "US",
                        PostalCode = "90265",
                        Latitude = 34.0259,
                        Longitude = -118.7798,
                    },
                ],
            };

            var roundTripped = entity.ToV2Dto().ToEntity();

            Assert.Equal(entity.Id, roundTripped.Id);
            Assert.Equal(entity.Name, roundTripped.Name);
            Assert.Equal(entity.Description, roundTripped.Description);
            Assert.Equal(entity.LitterReportStatusId, roundTripped.LitterReportStatusId);
            Assert.Equal(entity.CreatedByUserId, roundTripped.CreatedByUserId);
            Assert.NotNull(roundTripped.LitterImages);
            Assert.Single(roundTripped.LitterImages);
            var img = roundTripped.LitterImages.First();
            Assert.Equal(imageId, img.Id);
            Assert.Equal("https://blob.example.com/beach.jpg", img.AzureBlobURL);
            Assert.Equal("Malibu", img.City);
            Assert.Equal("CA", img.Region);
        }
    }
}
