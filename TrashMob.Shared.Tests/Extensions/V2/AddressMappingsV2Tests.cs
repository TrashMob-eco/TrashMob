namespace TrashMob.Shared.Tests.Extensions.V2
{
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using Xunit;

    public class AddressMappingsV2Tests
    {
        [Fact]
        public void Address_ToV2Dto_MapsAllProperties()
        {
            var entity = new Address
            {
                StreetAddress = "123 Main St",
                City = "Seattle",
                Region = "Washington",
                PostalCode = "98101",
                Country = "United States",
                County = "King",
            };

            var dto = entity.ToV2Dto();

            Assert.Equal("123 Main St", dto.StreetAddress);
            Assert.Equal("Seattle", dto.City);
            Assert.Equal("Washington", dto.Region);
            Assert.Equal("98101", dto.PostalCode);
            Assert.Equal("United States", dto.Country);
            Assert.Equal("King", dto.County);
        }

        [Fact]
        public void Address_ToV2Dto_HandlesNullStrings()
        {
            var entity = new Address
            {
                StreetAddress = null,
                City = null,
                Region = null,
                PostalCode = null,
                Country = null,
                County = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.StreetAddress);
            Assert.Equal(string.Empty, dto.City);
            Assert.Equal(string.Empty, dto.Region);
            Assert.Equal(string.Empty, dto.PostalCode);
            Assert.Equal(string.Empty, dto.Country);
            Assert.Equal(string.Empty, dto.County);
        }
    }
}
