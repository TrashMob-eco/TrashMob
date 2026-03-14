namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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

    public class UserFeedbackV2ControllerTests
    {
        private readonly Mock<IUserFeedbackManager> feedbackManager = new();
        private readonly Mock<ILogger<UserFeedbackV2Controller>> logger = new();
        private readonly UserFeedbackV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public UserFeedbackV2ControllerTests()
        {
            controller = new UserFeedbackV2Controller(feedbackManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task SubmitFeedback_ValidInput_Returns201()
        {
            var writeDto = new UserFeedbackWriteDto
            {
                Category = "Bug",
                Description = "Something is broken",
                Email = "test@example.com",
            };

            feedbackManager.Setup(m => m.AddAsync(It.IsAny<UserFeedback>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserFeedback
                {
                    Id = Guid.NewGuid(),
                    Category = "Bug",
                    Description = "Something is broken",
                    Email = "test@example.com",
                    Status = "New",
                });

            var result = await controller.SubmitFeedback(writeDto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
            var dto = Assert.IsType<UserFeedbackDto>(objectResult.Value);
            Assert.Equal("Bug", dto.Category);
            Assert.Equal("Something is broken", dto.Description);
        }

        [Fact]
        public async Task SubmitFeedback_MissingCategory_ReturnsBadRequest()
        {
            var writeDto = new UserFeedbackWriteDto
            {
                Category = "",
                Description = "Something is broken",
            };

            var result = await controller.SubmitFeedback(writeDto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task SubmitFeedback_InvalidCategory_ReturnsBadRequest()
        {
            var writeDto = new UserFeedbackWriteDto
            {
                Category = "InvalidCat",
                Description = "Something is broken",
            };

            var result = await controller.SubmitFeedback(writeDto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetFeedback_Found_ReturnsOk()
        {
            var feedbackId = Guid.NewGuid();
            feedbackManager.Setup(m => m.GetAsync(feedbackId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserFeedback
                {
                    Id = feedbackId,
                    Category = "General",
                    Description = "Nice app",
                    Status = "New",
                });

            var result = await controller.GetFeedback(feedbackId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserFeedbackDto>(okResult.Value);
            Assert.Equal(feedbackId, dto.Id);
        }

        [Fact]
        public async Task GetFeedback_NotFound_Returns404()
        {
            feedbackManager.Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserFeedback)null);

            var result = await controller.GetFeedback(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAllFeedback_ReturnsOkWithList()
        {
            var items = new List<UserFeedback>
            {
                new() { Id = Guid.NewGuid(), Category = "Bug", Description = "Bug 1", Status = "New" },
                new() { Id = Guid.NewGuid(), Category = "General", Description = "General 1", Status = "Reviewed" },
            };

            feedbackManager.Setup(m => m.GetByStatusAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(items);

            var result = await controller.GetAllFeedback();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<UserFeedbackDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
        }

        [Fact]
        public async Task UpdateFeedback_ValidInput_ReturnsOk()
        {
            var feedbackId = Guid.NewGuid();
            var updateDto = new UpdateFeedbackStatusDto
            {
                Status = "Reviewed",
                InternalNotes = "Looks good",
            };

            feedbackManager.Setup(m => m.UpdateStatusAsync(feedbackId, "Reviewed", "Looks good", testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserFeedback
                {
                    Id = feedbackId,
                    Category = "Bug",
                    Description = "Bug report",
                    Status = "Reviewed",
                    InternalNotes = "Looks good",
                });

            var result = await controller.UpdateFeedback(feedbackId, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserFeedbackDto>(okResult.Value);
            Assert.Equal("Reviewed", dto.Status);
        }

        [Fact]
        public async Task UpdateFeedback_NotFound_Returns404()
        {
            var updateDto = new UpdateFeedbackStatusDto { Status = "Reviewed" };
            feedbackManager.Setup(m => m.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserFeedback)null);

            var result = await controller.UpdateFeedback(Guid.NewGuid(), updateDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteFeedback_Found_ReturnsNoContent()
        {
            var feedbackId = Guid.NewGuid();
            feedbackManager.Setup(m => m.GetAsync(feedbackId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserFeedback { Id = feedbackId });

            var result = await controller.DeleteFeedback(feedbackId);

            Assert.IsType<NoContentResult>(result);
            feedbackManager.Verify(m => m.DeleteAsync(feedbackId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteFeedback_NotFound_Returns404()
        {
            feedbackManager.Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserFeedback)null);

            var result = await controller.DeleteFeedback(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
