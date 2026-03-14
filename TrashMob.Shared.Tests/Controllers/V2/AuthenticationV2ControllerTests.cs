namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class AuthenticationV2ControllerTests
    {
        private readonly Mock<IActiveDirectoryManager> activeDirectoryManager = new();
        private readonly Mock<ILogger<AuthenticationV2Controller>> logger = new();
        private readonly AuthenticationV2Controller controller;

        public AuthenticationV2ControllerTests()
        {
            controller = new AuthenticationV2Controller(
                activeDirectoryManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        #region ValidateUser

        [Fact]
        public async Task ValidateUser_ReturnsOk_WhenValid()
        {
            var request = new ActiveDirectoryValidateNewUserRequest { email = "test@example.com" };

            activeDirectoryManager
                .Setup(m => m.ValidateNewUserAsync(It.IsAny<ActiveDirectoryValidateNewUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ActiveDirectoryContinuationResponse { action = "Continue", version = "1.0.0" });

            var result = await controller.ValidateUser(request, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ValidateUser_ReturnsConflict_WhenValidationError()
        {
            var request = new ActiveDirectoryValidateNewUserRequest { email = "test@example.com" };

            activeDirectoryManager
                .Setup(m => m.ValidateNewUserAsync(It.IsAny<ActiveDirectoryValidateNewUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ActiveDirectoryValidationFailedResponse { action = "ValidationError", version = "1.0.0", userMessage = "exists" });

            var result = await controller.ValidateUser(request, CancellationToken.None);

            Assert.IsType<ConflictObjectResult>(result);
        }

        #endregion

        #region SignUpUser

        [Fact]
        public async Task SignUpUser_ReturnsOk_WhenValid()
        {
            var request = new ActiveDirectoryNewUserRequest { email = "test@example.com", objectId = Guid.NewGuid(), userName = "testuser" };

            activeDirectoryManager
                .Setup(m => m.CreateUserAsync(It.IsAny<ActiveDirectoryNewUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ActiveDirectoryContinuationResponse { action = "Continue", version = "1.0.0" });

            var result = await controller.SignUpUser(request, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }

        #endregion

        #region DeleteUser

        [Fact]
        public async Task DeleteUser_ReturnsOk()
        {
            var objectId = Guid.NewGuid();
            var request = new ActiveDirectoryDeleteUserRequest { objectId = objectId };

            activeDirectoryManager
                .Setup(m => m.DeleteUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ActiveDirectoryContinuationResponse { action = "Continue", version = "1.0.0" });

            var result = await controller.DeleteUser(request, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        #endregion
    }
}
