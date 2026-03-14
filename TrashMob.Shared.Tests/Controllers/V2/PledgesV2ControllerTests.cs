namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Contacts;
    using Xunit;

    public class PledgesV2ControllerTests
    {
        private readonly Mock<IPledgeManager> pledgeManager = new();
        private readonly Mock<ILogger<PledgesV2Controller>> logger = new();
        private readonly PledgesV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PledgesV2ControllerTests()
        {
            controller = new PledgesV2Controller(pledgeManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var pledges = new List<Pledge>
            {
                new() { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), TotalAmount = 500m, StartDate = DateTimeOffset.UtcNow },
                new() { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), TotalAmount = 1000m, StartDate = DateTimeOffset.UtcNow },
            };

            pledgeManager
                .Setup(m => m.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(pledges);

            var result = await controller.GetAll(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PledgeDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            pledgeManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Pledge)null);

            var result = await controller.GetById(id, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var pledge = new Pledge { Id = Guid.NewGuid(), ContactId = Guid.NewGuid(), TotalAmount = 500m, StartDate = DateTimeOffset.UtcNow };

            pledgeManager
                .Setup(m => m.AddAsync(It.IsAny<Pledge>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pledge);

            var dto = new PledgeDto { ContactId = pledge.ContactId, TotalAmount = 500m, StartDate = DateTimeOffset.UtcNow };

            var result = await controller.Create(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PledgeDto>(createdResult.Value);
            Assert.Equal(500m, resultDto.TotalAmount);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var pledgeId = Guid.NewGuid();

            pledgeManager
                .Setup(m => m.DeleteAsync(pledgeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(pledgeId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
