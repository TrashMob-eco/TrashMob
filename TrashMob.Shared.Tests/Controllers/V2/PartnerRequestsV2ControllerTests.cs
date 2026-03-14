namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class PartnerRequestsV2ControllerTests
    {
        private readonly Mock<IPartnerRequestManager> partnerRequestManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerRequestsV2Controller>> logger = new();
        private readonly PartnerRequestsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerRequestsV2ControllerTests()
        {
            controller = new PartnerRequestsV2Controller(
                partnerRequestManager.Object,
                authorizationService.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task Add_ValidInput_Returns201()
        {
            var dto = new PartnerRequestDto
            {
                Name = "New Partner Org",
                Email = "partner@test.com",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PartnerTypeId = 1,
            };
            var created = new PartnerRequest
            {
                Id = Guid.NewGuid(),
                Name = "New Partner Org",
                Email = "partner@test.com",
                City = "Seattle",
                Region = "WA",
                Country = "US",
                PartnerTypeId = 1,
            };

            partnerRequestManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerRequest>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await controller.Add(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PartnerRequestDto>(createdResult.Value);
            Assert.Equal("New Partner Org", resultDto.Name);
        }

        [Fact]
        public async Task Approve_ReturnsOk()
        {
            var requestId = Guid.NewGuid();
            var approved = new PartnerRequest
            {
                Id = requestId,
                Name = "Approved Partner",
                PartnerRequestStatusId = 2,
            };

            partnerRequestManager
                .Setup(m => m.ApproveBecomeAPartnerAsync(requestId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(approved);

            var result = await controller.Approve(requestId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerRequestDto>(okResult.Value);
            Assert.Equal("Approved Partner", dto.Name);
        }

        [Fact]
        public async Task Deny_ReturnsOk()
        {
            var requestId = Guid.NewGuid();
            var denied = new PartnerRequest
            {
                Id = requestId,
                Name = "Denied Partner",
                PartnerRequestStatusId = 3,
            };

            partnerRequestManager
                .Setup(m => m.DenyBecomeAPartnerAsync(requestId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(denied);

            var result = await controller.Deny(requestId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerRequestDto>(okResult.Value);
            Assert.Equal("Denied Partner", dto.Name);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var requests = new List<PartnerRequest>
            {
                new() { Id = Guid.NewGuid(), Name = "Partner A", PartnerRequestStatusId = 1, PartnerTypeId = 1 },
                new() { Id = Guid.NewGuid(), Name = "Partner B", PartnerRequestStatusId = 2, PartnerTypeId = 2 },
            };

            partnerRequestManager
                .Setup(m => m.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(requests);

            var result = await controller.GetAll(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerRequestDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenFound()
        {
            var requestId = Guid.NewGuid();
            var request = new PartnerRequest
            {
                Id = requestId,
                Name = "Test Partner",
                Email = "test@test.com",
                PartnerTypeId = 1,
            };

            partnerRequestManager
                .Setup(m => m.GetAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(request);

            var result = await controller.Get(requestId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerRequestDto>(okResult.Value);
            Assert.Equal("Test Partner", dto.Name);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenMissing()
        {
            var requestId = Guid.NewGuid();

            partnerRequestManager
                .Setup(m => m.GetAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerRequest)null);

            var result = await controller.Get(requestId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByUser_ReturnsOkWithList()
        {
            var userId = Guid.NewGuid();
            var requests = new List<PartnerRequest>
            {
                new() { Id = Guid.NewGuid(), Name = "My Request", PartnerTypeId = 1 },
            };

            partnerRequestManager
                .Setup(m => m.GetByCreatedUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(requests);

            var result = await controller.GetByUser(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerRequestDto>>(okResult.Value);
            Assert.Single(dtos);
        }
    }
}
