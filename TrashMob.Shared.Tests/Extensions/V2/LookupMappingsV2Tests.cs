namespace TrashMob.Shared.Tests.Extensions.V2
{
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class LookupMappingsV2Tests
    {
        [Fact]
        public void LookupModel_ToV2Dto_MapsAllProperties()
        {
            var entity = new EventType
            {
                Id = 3,
                Name = "Park Cleanup",
                Description = "Clean up a local park",
                DisplayOrder = 2,
                IsActive = true,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(3, dto.Id);
            Assert.Equal("Park Cleanup", dto.Name);
            Assert.Equal("Clean up a local park", dto.Description);
            Assert.Equal(2, dto.DisplayOrder);
        }

        [Fact]
        public void LookupModel_ToV2Dto_HandlesNullStrings()
        {
            var entity = new ServiceType
            {
                Id = 1,
                Name = null,
                Description = null,
                DisplayOrder = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(1, dto.Id);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Description);
            Assert.Equal(0, dto.DisplayOrder);
        }
    }
}
