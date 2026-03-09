namespace TrashMob.Shared.Tests.Extensions.V2
{
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using Xunit;

    public class ContactRequestMappingsV2Tests
    {
        [Fact]
        public void ContactRequestDto_ToEntity_MapsAllProperties()
        {
            var dto = new ContactRequestDto
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Message = "I'd like to volunteer!",
            };

            var entity = dto.ToEntity();

            Assert.Equal("Jane Doe", entity.Name);
            Assert.Equal("jane@example.com", entity.Email);
            Assert.Equal("I'd like to volunteer!", entity.Message);
        }

        [Fact]
        public void ContactRequestDto_ToEntity_HandlesEmptyStrings()
        {
            var dto = new ContactRequestDto();

            var entity = dto.ToEntity();

            Assert.Equal(string.Empty, entity.Name);
            Assert.Equal(string.Empty, entity.Email);
            Assert.Equal(string.Empty, entity.Message);
        }
    }
}
