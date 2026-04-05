namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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

    public class DependentsV2ControllerTests
    {
        private readonly Mock<IDependentManager> dependentManager = new();
        private readonly Mock<IDependentWaiverManager> dependentWaiverManager = new();
        private readonly Mock<ILogger<DependentsV2Controller>> logger = new();
        private readonly DependentsV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public DependentsV2ControllerTests()
        {
            controller = new DependentsV2Controller(dependentManager.Object, dependentWaiverManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task GetDependents_ReturnsOk_WhenOwner()
        {
            var dependents = new List<Dependent>
            {
                new() { Id = Guid.NewGuid(), ParentUserId = userId, FirstName = "Jane", LastName = "Doe", IsActive = true },
            };

            dependentManager.Setup(m => m.GetByParentUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependents);

            var result = await controller.GetDependents(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<DependentDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Jane", dtos[0].FirstName);
        }

        [Fact]
        public async Task GetDependents_ReturnsForbid_WhenNotOwner()
        {
            var result = await controller.GetDependents(Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Add_ReturnsCreated_WhenOwner()
        {
            var dto = new DependentDto
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = new DateOnly(2015, 1, 1),
                Relationship = "Child",
            };

            dependentManager.Setup(m => m.AddAsync(It.IsAny<Dependent>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dependent
                {
                    Id = Guid.NewGuid(),
                    ParentUserId = userId,
                    FirstName = "John",
                    LastName = "Smith",
                    IsActive = true,
                });

            var result = await controller.Add(userId, dto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwner()
        {
            var dependentId = Guid.NewGuid();
            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dependent { Id = dependentId, ParentUserId = userId });
            dependentManager.Setup(m => m.SoftDeleteAsync(dependentId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(userId, dependentId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenDependentMissing()
        {
            dependentManager.Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dependent)null);

            var result = await controller.Delete(userId, Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsForbid_WhenNotOwner()
        {
            var result = await controller.Update(Guid.NewGuid(), Guid.NewGuid(), new DependentDto(), CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
