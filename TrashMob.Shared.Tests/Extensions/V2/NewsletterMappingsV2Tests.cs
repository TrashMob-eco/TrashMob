namespace TrashMob.Shared.Tests.Extensions.V2
{
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class NewsletterMappingsV2Tests
    {
        [Fact]
        public void NewsletterCategory_ToV2Dto_MapsAllProperties()
        {
            var entity = new NewsletterCategory
            {
                Id = 5,
                Name = "Monthly Digest",
                Description = "A monthly summary of activities",
                IsDefault = true,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(5, dto.Id);
            Assert.Equal("Monthly Digest", dto.Name);
            Assert.Equal("A monthly summary of activities", dto.Description);
            Assert.True(dto.IsDefault);
        }

        [Fact]
        public void NewsletterCategory_ToV2Dto_HandlesNullStrings()
        {
            var entity = new NewsletterCategory
            {
                Id = 1,
                Name = null,
                Description = null,
                IsDefault = false,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Description);
            Assert.False(dto.IsDefault);
        }
    }
}
