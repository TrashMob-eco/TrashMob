namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class WaiverMappingsV2Tests
    {
        [Fact]
        public void WaiverVersion_ToV2Dto_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var effectiveDate = DateTimeOffset.UtcNow.AddDays(-30);
            var expiryDate = DateTimeOffset.UtcNow.AddDays(335);

            var entity = new WaiverVersion
            {
                Id = id,
                Name = "Standard Waiver",
                Version = "1.0",
                WaiverText = "By signing...",
                EffectiveDate = effectiveDate,
                ExpiryDate = expiryDate,
                IsActive = true,
                Scope = WaiverScope.Global,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(id, dto.Id);
            Assert.Equal("Standard Waiver", dto.Name);
            Assert.Equal("1.0", dto.Version);
            Assert.Equal("By signing...", dto.WaiverText);
            Assert.Equal(effectiveDate, dto.EffectiveDate);
            Assert.Equal(expiryDate, dto.ExpiryDate);
            Assert.True(dto.IsActive);
            Assert.Equal(WaiverScope.Global, dto.Scope);
        }

        [Fact]
        public void WaiverVersion_ToV2Dto_HandlesNullStrings()
        {
            var entity = new WaiverVersion
            {
                Id = Guid.NewGuid(),
                Name = null,
                Version = null,
                WaiverText = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Version);
            Assert.Equal(string.Empty, dto.WaiverText);
        }

        [Fact]
        public void UserWaiver_ToV2Dto_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var waiverVersionId = Guid.NewGuid();
            var acceptedDate = DateTimeOffset.UtcNow.AddDays(-7);
            var expiryDate = DateTimeOffset.UtcNow.AddDays(358);

            var entity = new UserWaiver
            {
                Id = id,
                UserId = userId,
                WaiverVersionId = waiverVersionId,
                AcceptedDate = acceptedDate,
                ExpiryDate = expiryDate,
                TypedLegalName = "Jane Doe",
                SigningMethod = "Digital",
                DocumentUrl = "https://example.com/waiver.pdf",
                IsMinor = true,
                GuardianName = "John Doe",
                GuardianRelationship = "Parent",
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(id, dto.Id);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(waiverVersionId, dto.WaiverVersionId);
            Assert.Equal(acceptedDate, dto.AcceptedDate);
            Assert.Equal(expiryDate, dto.ExpiryDate);
            Assert.Equal("Jane Doe", dto.TypedLegalName);
            Assert.Equal("Digital", dto.SigningMethod);
            Assert.Equal("https://example.com/waiver.pdf", dto.DocumentUrl);
            Assert.True(dto.IsMinor);
            Assert.Equal("John Doe", dto.GuardianName);
            Assert.Equal("Parent", dto.GuardianRelationship);
        }

        [Fact]
        public void UserWaiver_ToV2Dto_HandlesNullStrings()
        {
            var entity = new UserWaiver
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                WaiverVersionId = Guid.NewGuid(),
                TypedLegalName = null,
                SigningMethod = null,
                DocumentUrl = null,
                GuardianName = null,
                GuardianRelationship = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.TypedLegalName);
            Assert.Equal(string.Empty, dto.SigningMethod);
            Assert.Equal(string.Empty, dto.DocumentUrl);
            Assert.Equal(string.Empty, dto.GuardianName);
            Assert.Equal(string.Empty, dto.GuardianRelationship);
        }
    }
}
