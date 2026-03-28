namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class ParentalConsentMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllFields()
        {
            var entity = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ParentUserId = Guid.NewGuid(),
                DependentId = Guid.NewGuid(),
                ConsentType = ConsentType.ParentInitiatedChild,
                Status = ConsentStatus.Verified,
                ConsentUrl = "https://consent-svc.privo.com/consent/123",
                VerifiedDate = new DateTimeOffset(2026, 3, 28, 12, 0, 0, TimeSpan.Zero),
                CreatedDate = new DateTimeOffset(2026, 3, 27, 10, 0, 0, TimeSpan.Zero),
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.UserId, dto.UserId);
            Assert.Equal(entity.ParentUserId, dto.ParentUserId);
            Assert.Equal(entity.DependentId, dto.DependentId);
            Assert.Equal((int)ConsentType.ParentInitiatedChild, dto.ConsentType);
            Assert.Equal((int)ConsentStatus.Verified, dto.Status);
            Assert.Equal("https://consent-svc.privo.com/consent/123", dto.ConsentUrl);
            Assert.Equal(entity.VerifiedDate, dto.VerifiedDate);
            Assert.Equal(entity.CreatedDate, dto.CreatedDate);
        }

        [Fact]
        public void ToV2Dto_HandlesNullConsentUrl()
        {
            var entity = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ConsentType = ConsentType.AdultVerification,
                Status = ConsentStatus.Pending,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.ConsentUrl);
        }

        [Fact]
        public void ToV2Dto_MapsEnumValues_Correctly()
        {
            var entity = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ConsentType = ConsentType.ChildInitiated,
                Status = ConsentStatus.Revoked,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(3, dto.ConsentType);
            Assert.Equal(5, dto.Status);
        }
    }
}
