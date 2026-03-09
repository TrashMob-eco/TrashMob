namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class EventPhotoMappingsV2Tests
    {
        [Fact]
        public void EventPhoto_ToV2Dto_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var takenAt = DateTimeOffset.UtcNow.AddHours(-1);
            var uploadedDate = DateTimeOffset.UtcNow;

            var entity = new EventPhoto
            {
                Id = id,
                EventId = eventId,
                UploadedByUserId = userId,
                ImageUrl = "https://example.com/photo.jpg",
                ThumbnailUrl = "https://example.com/thumb.jpg",
                PhotoType = EventPhotoType.During,
                Caption = "Test caption",
                TakenAt = takenAt,
                UploadedDate = uploadedDate,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(id, dto.Id);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(userId, dto.UploadedByUserId);
            Assert.Equal("https://example.com/photo.jpg", dto.ImageUrl);
            Assert.Equal("https://example.com/thumb.jpg", dto.ThumbnailUrl);
            Assert.Equal(EventPhotoType.During, dto.PhotoType);
            Assert.Equal("Test caption", dto.Caption);
            Assert.Equal(takenAt, dto.TakenAt);
            Assert.Equal(uploadedDate, dto.UploadedDate);
        }

        [Fact]
        public void EventPhoto_ToV2Dto_HandlesNullStrings()
        {
            var entity = new EventPhoto
            {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                ImageUrl = null,
                ThumbnailUrl = null,
                Caption = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.ImageUrl);
            Assert.Equal(string.Empty, dto.ThumbnailUrl);
            Assert.Equal(string.Empty, dto.Caption);
        }
    }
}
