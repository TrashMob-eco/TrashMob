namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class EventAttendeeMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var entity = new EventAttendee
            {
                UserId = Guid.NewGuid(),
                SignUpDate = new DateTimeOffset(2026, 3, 1, 10, 0, 0, TimeSpan.Zero),
                IsEventLead = true,
                User = new User
                {
                    UserName = "cleanupking",
                    GivenName = "Alex",
                    ProfilePhotoUrl = "https://example.com/photo.jpg",
                },
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.UserId, dto.UserId);
            Assert.Equal("cleanupking", dto.UserName);
            Assert.Equal("Alex", dto.GivenName);
            Assert.Equal("https://example.com/photo.jpg", dto.ProfilePhotoUrl);
            Assert.Equal(entity.SignUpDate, dto.SignUpDate);
            Assert.True(dto.IsEventLead);
        }

        [Fact]
        public void ToV2Dto_HandlesNullUser()
        {
            var entity = new EventAttendee
            {
                UserId = Guid.NewGuid(),
                SignUpDate = DateTimeOffset.UtcNow,
                IsEventLead = false,
                User = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(entity.UserId, dto.UserId);
            Assert.Equal(string.Empty, dto.UserName);
            Assert.Equal(string.Empty, dto.GivenName);
            Assert.Equal(string.Empty, dto.ProfilePhotoUrl);
        }
    }
}
