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

    public class MessageRequestV2ControllerTests
    {
        private readonly Mock<IMessageRequestManager> messageRequestManager = new();
        private readonly Mock<ILogger<MessageRequestV2Controller>> logger = new();
        private readonly MessageRequestV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public MessageRequestV2ControllerTests()
        {
            controller = new MessageRequestV2Controller(messageRequestManager.Object, logger.Object);

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
        public async Task SendMessageRequest_ReturnsOk()
        {
            var dto = new MessageRequestDto
            {
                Id = Guid.NewGuid(),
                Name = "Weekly Update",
                Message = "Hello volunteers!",
            };

            messageRequestManager
                .Setup(m => m.SendMessageRequestAsync(It.IsAny<MessageRequest>()))
                .Returns(Task.CompletedTask);

            var result = await controller.SendMessageRequest(dto);

            Assert.IsType<OkResult>(result);
            messageRequestManager.Verify(
                m => m.SendMessageRequestAsync(It.IsAny<MessageRequest>()),
                Times.Once);
        }

        [Fact]
        public async Task SendMessageRequest_VerifiesEntityMapping()
        {
            var dto = new MessageRequestDto
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Message = "Hello",
            };

            messageRequestManager
                .Setup(m => m.SendMessageRequestAsync(It.IsAny<MessageRequest>()))
                .Returns(Task.CompletedTask);

            await controller.SendMessageRequest(dto);

            messageRequestManager.Verify(
                m => m.SendMessageRequestAsync(
                    It.Is<MessageRequest>(e =>
                        e.Name == "Test" &&
                        e.Message == "Hello")),
                Times.Once);
        }
    }
}
