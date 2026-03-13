namespace TrashMob.Shared.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using TrashMob.Models.Poco.V2;
    using Xunit;

    public class LitterReportDtoSerializationTests
    {
        /// <summary>
        /// Server JSON options matching Program.cs AddJsonOptions configuration.
        /// Note: GeoJsonConverterFactory excluded since it requires NTS package not in test project,
        /// but it only handles NetTopologySuite.Geometries types, not plain DTOs.
        /// </summary>
        private static JsonSerializerOptions ServerOptions => new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            PropertyNameCaseInsensitive = true,
        };

        /// <summary>
        /// Mobile JSON options matching RestServiceBase.SerializerOptions.
        /// </summary>
        private static readonly JsonSerializerOptions MobileOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        private static readonly JsonSerializerOptions WebOptions = new(JsonSerializerDefaults.Web);

        /// <summary>
        /// Server options WITHOUT PropertyNameCaseInsensitive - matches what AddJsonOptions
        /// produces when starting from non-Web defaults.
        /// </summary>
        private static readonly JsonSerializerOptions StrictServerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        };

        [Fact]
        public void MobileSerialized_ServerDeserialized_ImagesPreserved()
        {
            // Arrange - create DTO as mobile would
            var dto = new LitterReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Report",
                Description = "Test Description",
                LitterReportStatusId = 3, // Cleaned
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Images =
                [
                    new LitterImageDto
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "https://example.com/image.jpg",
                        StreetAddress = "123 Main St",
                        City = "Seattle",
                        Region = "WA",
                        Country = "US",
                        PostalCode = "98101",
                        Latitude = 47.6,
                        Longitude = -122.3,
                    },
                ],
            };

            // Act - serialize with mobile options, deserialize with server options
            var json = JsonSerializer.Serialize(dto, MobileOptions);
            var deserialized = JsonSerializer.Deserialize<LitterReportDto>(json, ServerOptions);

            // Assert
            Assert.NotNull(deserialized);
            Assert.NotNull(deserialized.Images);
            Assert.Single(deserialized.Images);
            Assert.Equal(dto.Images[0].Id, deserialized.Images[0].Id);
            Assert.Equal(dto.Images[0].ImageUrl, deserialized.Images[0].ImageUrl);
        }

        [Fact]
        public void MobileSerialized_DefaultDeserialized_ImagesPreserved()
        {
            // Arrange
            var dto = new LitterReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Report",
                Description = "Test",
                LitterReportStatusId = 1,
                CreatedByUserId = Guid.NewGuid(),
                Images =
                [
                    new LitterImageDto
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "https://example.com/image.jpg",
                        City = "Seattle",
                    },
                ],
            };

            // Act - serialize with mobile options, deserialize with DEFAULT options (no case insensitivity)
            var json = JsonSerializer.Serialize(dto, MobileOptions);
            var defaultDeserialized = JsonSerializer.Deserialize<LitterReportDto>(json);

            // Assert - DEFAULT options do NOT have PropertyNameCaseInsensitive
            // camelCase "images" won't match PascalCase "Images"
            Assert.NotNull(defaultDeserialized);
            // This reveals if the default (non-Web) options fail to deserialize
            var imageCount = defaultDeserialized.Images?.Count ?? 0;

            // Now test with Web defaults (what ASP.NET Core actually uses)
            var webDeserialized = JsonSerializer.Deserialize<LitterReportDto>(json, WebOptions);
            Assert.NotNull(webDeserialized);
            Assert.Single(webDeserialized.Images);
        }

        [Fact]
        public void VerifyMobileSerializationProducesImages()
        {
            // Arrange
            var dto = new LitterReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Description = "Test",
                LitterReportStatusId = 1,
                CreatedByUserId = Guid.NewGuid(),
                Images =
                [
                    new LitterImageDto { Id = Guid.NewGuid(), ImageUrl = "https://example.com/1.jpg" },
                    new LitterImageDto { Id = Guid.NewGuid(), ImageUrl = "https://example.com/2.jpg" },
                ],
            };

            // Act
            var json = JsonSerializer.Serialize(dto, MobileOptions);

            // Assert - verify the JSON actually contains the images
            Assert.Contains("\"images\"", json);
            Assert.Contains("\"imageUrl\"", json);

            // Parse and count
            var doc = JsonDocument.Parse(json);
            var imagesArray = doc.RootElement.GetProperty("images");
            Assert.Equal(2, imagesArray.GetArrayLength());
        }

        [Fact]
        public void ServerOptions_WithoutCaseInsensitive_FailsToDeserializeCamelCase()
        {
            // This test proves the bug: if PropertyNameCaseInsensitive is not set,
            // camelCase JSON won't map to PascalCase C# properties
            var dto = new LitterReportDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Description = "Test",
                LitterReportStatusId = 1,
                CreatedByUserId = Guid.NewGuid(),
                Images = [new LitterImageDto { Id = Guid.NewGuid(), ImageUrl = "url" }],
            };

            var json = JsonSerializer.Serialize(dto, MobileOptions);

            var deserialized = JsonSerializer.Deserialize<LitterReportDto>(json, StrictServerOptions);

            // With strict case matching, camelCase "images" won't match "Images"
            Assert.NotNull(deserialized);
            Assert.Empty(deserialized.Images); // Images will be empty!
            Assert.Equal(Guid.Empty, deserialized.Id); // Even Id won't match "id"!
        }
    }
}
