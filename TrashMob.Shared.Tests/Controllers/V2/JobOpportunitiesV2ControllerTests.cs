namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
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
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class JobOpportunitiesV2ControllerTests
    {
        private readonly Mock<IKeyedManager<JobOpportunity>> jobOpportunityManager = new();
        private readonly Mock<ILogger<JobOpportunitiesV2Controller>> logger = new();
        private readonly JobOpportunitiesV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public JobOpportunitiesV2ControllerTests()
        {
            controller = new JobOpportunitiesV2Controller(jobOpportunityManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([], "Bearer")),
            };
            httpContext.Items["UserId"] = userId.ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var entity = new JobOpportunity { Id = id, Title = "Test Job" };

            jobOpportunityManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await controller.Get(id, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();

            jobOpportunityManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobOpportunity)null);

            var result = await controller.Get(id, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var dto = new JobOpportunityDto { Title = "New Job" };
            var created = new JobOpportunity { Id = Guid.NewGuid(), Title = "New Job" };

            jobOpportunityManager
                .Setup(m => m.AddAsync(It.IsAny<JobOpportunity>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await controller.Create(dto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var existing = new JobOpportunity { Id = id, Title = "Old Job" };
            var updated = new JobOpportunity { Id = id, Title = "Updated Job" };
            var dto = new JobOpportunityDto { Title = "Updated Job" };

            jobOpportunityManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            jobOpportunityManager
                .Setup(m => m.UpdateAsync(It.IsAny<JobOpportunity>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updated);

            var result = await controller.Update(id, dto, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenFound()
        {
            var id = Guid.NewGuid();
            var existing = new JobOpportunity { Id = id, Title = "To Delete" };

            jobOpportunityManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            jobOpportunityManager
                .Setup(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
