namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Shared.Persistence.Interfaces;
    using Xunit;

    public class SecretsV2ControllerTests
    {
        private readonly Mock<ISecretRepository> secretRepository = new();
        private readonly Mock<ILogger<SecretsV2Controller>> logger = new();
        private readonly SecretsV2Controller controller;

        public SecretsV2ControllerTests()
        {
            controller = new SecretsV2Controller(secretRepository.Object, logger.Object);

            var userId = Guid.NewGuid();
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
        public async Task GetSecret_ReturnsOk_WithSecretValue()
        {
            secretRepository
                .Setup(r => r.Get("test-name"))
                .Returns("test-value");

            var result = await controller.GetSecret("test-name");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("test-value", okResult.Value);
        }
    }
}
