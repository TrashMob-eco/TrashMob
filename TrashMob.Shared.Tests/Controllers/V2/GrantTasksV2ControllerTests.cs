namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class GrantTasksV2ControllerTests
    {
        private readonly Mock<IGrantTaskManager> grantTaskManager = new();
        private readonly Mock<ILogger<GrantTasksV2Controller>> logger = new();
        private readonly GrantTasksV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public GrantTasksV2ControllerTests()
        {
            controller = new GrantTasksV2Controller(grantTaskManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetByGrantId_ReturnsOk()
        {
            var grantId = Guid.NewGuid();
            var tasks = new List<GrantTask>
            {
                new GrantTask { Id = Guid.NewGuid(), GrantId = grantId, Title = "Submit application" },
                new GrantTask { Id = Guid.NewGuid(), GrantId = grantId, Title = "Gather documents" },
            };

            grantTaskManager
                .Setup(m => m.GetByGrantIdAsync(grantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tasks);

            var result = await controller.GetByGrantId(grantId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var grantId = Guid.NewGuid();
            var taskDto = new GrantTaskDto { Id = Guid.NewGuid(), GrantId = grantId, Title = "Submit application" };
            var grantTask = new GrantTask { Id = taskDto.Id, GrantId = grantId, Title = "Submit application" };

            grantTaskManager
                .Setup(m => m.AddAsync(It.IsAny<GrantTask>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grantTask);

            var result = await controller.Create(taskDto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            grantTaskManager
                .Setup(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
