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
        private readonly Mock<ILookupManager<EventStatus>> eventStatusManager = new();
        private readonly Mock<ILookupManager<ServiceType>> serviceTypeManager = new();
        private readonly Mock<ILookupManager<EventPartnerLocationServiceStatus>> partnerServiceStatusManager = new();
        private readonly Mock<ILookupManager<PartnerType>> partnerTypeManager = new();
        private readonly Mock<ILookupManager<PartnerStatus>> partnerStatusManager = new();
        private readonly Mock<ILookupManager<PartnerRequestStatus>> partnerRequestStatusManager = new();
        private readonly Mock<ILookupManager<WeightUnit>> weightUnitManager = new();
        private readonly Mock<ILookupManager<InvitationStatus>> invitationStatusManager = new();
        private readonly Mock<ILookupManager<SocialMediaAccountType>> socialMediaAccountTypeManager = new();
        private readonly Mock<ILogger<LookupsV2Controller>> logger = new();
        private readonly LookupsV2Controller controller;

        public LookupsV2ControllerTests()
        {
            controller = new LookupsV2Controller(
                eventTypeManager.Object,
                eventStatusManager.Object,
                serviceTypeManager.Object,
                partnerServiceStatusManager.Object,
                partnerTypeManager.Object,
                partnerStatusManager.Object,
                partnerRequestStatusManager.Object,
                weightUnitManager.Object,
                invitationStatusManager.Object,
                socialMediaAccountTypeManager.Object,
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

        [Fact]
        public async Task GetEventStatuses_ReturnsOkWithList()
        {
            var statuses = new List<EventStatus>
            {
                new() { Id = 1, Name = "Active", Description = "Event is active", DisplayOrder = 1 },
                new() { Id = 2, Name = "Cancelled", Description = "Event is cancelled", DisplayOrder = 2 },
            };

            eventStatusManager.Setup(m => m.GetAsync()).ReturnsAsync(statuses);

            var result = await controller.GetEventStatuses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Active", dtos[0].Name);
            Assert.Equal("Cancelled", dtos[1].Name);
        }

        [Fact]
        public async Task GetPartnerTypes_ReturnsOkWithList()
        {
            var types = new List<PartnerType>
            {
                new() { Id = 1, Name = "Government", Description = "Government entity", DisplayOrder = 1 },
                new() { Id = 2, Name = "Business", Description = "Business partner", DisplayOrder = 2 },
            };

            partnerTypeManager.Setup(m => m.GetAsync()).ReturnsAsync(types);

            var result = await controller.GetPartnerTypes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Government", dtos[0].Name);
            Assert.Equal("Business", dtos[1].Name);
        }

        [Fact]
        public async Task GetPartnerStatuses_ReturnsOkWithList()
        {
            var statuses = new List<PartnerStatus>
            {
                new() { Id = 1, Name = "Active", Description = "Active partner", DisplayOrder = 1 },
                new() { Id = 2, Name = "Inactive", Description = "Inactive partner", DisplayOrder = 2 },
            };

            partnerStatusManager.Setup(m => m.GetAsync()).ReturnsAsync(statuses);

            var result = await controller.GetPartnerStatuses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Active", dtos[0].Name);
            Assert.Equal("Inactive", dtos[1].Name);
        }

        [Fact]
        public async Task GetPartnerRequestStatuses_ReturnsOkWithList()
        {
            var statuses = new List<PartnerRequestStatus>
            {
                new() { Id = 1, Name = "Pending", Description = "Request pending", DisplayOrder = 1 },
                new() { Id = 2, Name = "Approved", Description = "Request approved", DisplayOrder = 2 },
            };

            partnerRequestStatusManager.Setup(m => m.GetAsync()).ReturnsAsync(statuses);

            var result = await controller.GetPartnerRequestStatuses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Pending", dtos[0].Name);
            Assert.Equal("Approved", dtos[1].Name);
        }

        [Fact]
        public async Task GetWeightUnits_ReturnsOkWithList()
        {
            var units = new List<WeightUnit>
            {
                new() { Id = 1, Name = "Pounds", Description = "Weight in pounds", DisplayOrder = 1 },
                new() { Id = 2, Name = "Kilograms", Description = "Weight in kilograms", DisplayOrder = 2 },
            };

            weightUnitManager.Setup(m => m.GetAsync()).ReturnsAsync(units);

            var result = await controller.GetWeightUnits();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Pounds", dtos[0].Name);
            Assert.Equal("Kilograms", dtos[1].Name);
        }

        [Fact]
        public async Task GetInvitationStatuses_ReturnsOkWithList()
        {
            var statuses = new List<InvitationStatus>
            {
                new() { Id = 1, Name = "Pending", Description = "Invitation pending", DisplayOrder = 1 },
                new() { Id = 2, Name = "Accepted", Description = "Invitation accepted", DisplayOrder = 2 },
            };

            invitationStatusManager.Setup(m => m.GetAsync()).ReturnsAsync(statuses);

            var result = await controller.GetInvitationStatuses();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Pending", dtos[0].Name);
            Assert.Equal("Accepted", dtos[1].Name);
        }

        [Fact]
        public async Task GetSocialMediaAccountTypes_ReturnsOkWithList()
        {
            var types = new List<SocialMediaAccountType>
            {
                new() { Id = 1, Name = "Facebook", Description = "Facebook account", DisplayOrder = 1 },
                new() { Id = 2, Name = "Instagram", Description = "Instagram account", DisplayOrder = 2 },
            };

            socialMediaAccountTypeManager.Setup(m => m.GetAsync()).ReturnsAsync(types);

            var result = await controller.GetSocialMediaAccountTypes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<LookupItemDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Facebook", dtos[0].Name);
            Assert.Equal("Instagram", dtos[1].Name);
        }
    }
}
