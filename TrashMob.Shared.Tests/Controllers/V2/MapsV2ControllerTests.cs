namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class MapsV2ControllerTests
    {
        private readonly Mock<IMapManager> mapManager = new();
        private readonly Mock<ILogger<MapsV2Controller>> logger = new();
        private readonly MapsV2Controller controller;

        public MapsV2ControllerTests()
        {
            controller = new MapsV2Controller(mapManager.Object, logger.Object);
        }

        [Fact]
        public async Task GetAddress_ReturnsOkWithAddressDto()
        {
            var address = new Address
            {
                StreetAddress = "123 Main St",
                City = "Seattle",
                Region = "Washington",
                PostalCode = "98101",
                Country = "United States",
                County = "King",
            };

            mapManager
                .Setup(m => m.GetAddressAsync(47.6, -122.3))
                .ReturnsAsync(address);

            var result = await controller.GetAddress(47.6, -122.3);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<AddressDto>(okResult.Value);
            Assert.Equal("123 Main St", dto.StreetAddress);
            Assert.Equal("Seattle", dto.City);
            Assert.Equal("Washington", dto.Region);
            Assert.Equal("98101", dto.PostalCode);
            Assert.Equal("United States", dto.Country);
            Assert.Equal("King", dto.County);
        }
    }
}
