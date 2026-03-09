namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class LookupsV2ControllerTests
    {
        private readonly Mock<ILookupManager<EventType>> eventTypeManager = new();
        private readonly Mock<ILookupManager<ServiceType>> serviceTypeManager = new();
        private readonly Mock<ILookupManager<EventPartnerLocationServiceStatus>> partnerServiceStatusManager = new();
        private readonly Mock<ILogger<LookupsV2Controller>> logger = new();
        private readonly LookupsV2Controller controller;

        public LookupsV2ControllerTests()
        {
            controller = new LookupsV2Controller(
                eventTypeManager.Object,
                serviceTypeManager.Object,
                partnerServiceStatusManager.Object,
                logger.Object);
        }

        [Fact]
        public async Task GetEventTypes_ReturnsOkWithList()
        {
            var types = new List<EventType>
            {
                new() { Id = 1, Name = "Park Cleanup", Description = "Clean up a park", DisplayOrder = 1 },
                new() { Id = 2, Name = "Beach Cleanup", Description = "Clean up a beach", DisplayOrder = 2 },
            };

            eventTypeManager.Setup(m => m.GetAsync()).ReturnsAsync(types);

            var result = await controller.GetEventTypes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Park Cleanup", dtos[0].Name);
            Assert.Equal("Beach Cleanup", dtos[1].Name);
        }

        [Fact]
        public async Task GetServiceTypes_ReturnsOkWithList()
        {
            var types = new List<ServiceType>
            {
                new() { Id = 1, Name = "Hauling", Description = "Waste hauling service", DisplayOrder = 1 },
            };

            serviceTypeManager.Setup(m => m.GetAsync()).ReturnsAsync(types);

            var result = await controller.GetServiceTypes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Hauling", dtos[0].Name);
        }

        [Fact]
        public async Task GetPartnerServiceStatuses_ReturnsOkWithList()
        {
            var statuses = new List<EventPartnerLocationServiceStatus>
            {
                new() { Id = 1, Name = "Requested", Description = "Service requested", DisplayOrder = 1 },
                new() { Id = 2, Name = "Accepted", Description = "Service accepted", DisplayOrder = 2 },
            };

            partnerServiceStatusManager.Setup(m => m.GetAsync()).ReturnsAsync(statuses);

            var result = await controller.GetPartnerServiceStatuses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Requested", dtos[0].Name);
            Assert.Equal("Accepted", dtos[1].Name);
        }
    }
}
