namespace TrashMob.Shared.Tests.Security
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using Xunit;

    public class UserOwnsEntityAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<ILogger<UserIsValidUserAuthHandler>> _mockLogger;
        private readonly UserOwnsEntityAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserOwnsEntityAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockLogger = new Mock<ILogger<UserIsValidUserAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserOwnsEntityAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserOwnsEntity_Succeeds()
        {
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("joe@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new Event { CreatedByUserId = userId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("joe@test.com");
            var requirement = new UserOwnsEntityRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserDoesNotOwnEntity_DoesNotSucceed()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("joe@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new Event { CreatedByUserId = otherUserId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("joe@test.com");
            var requirement = new UserOwnsEntityRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var resource = new Event { CreatedByUserId = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("missing@test.com");
            var requirement = new UserOwnsEntityRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_ValidUser_SetsUserIdInHttpContext()
        {
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("joe@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new Event { CreatedByUserId = userId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("joe@test.com");
            var requirement = new UserOwnsEntityRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.Equal(userId, _mockHttpContextAccessor.Object.HttpContext.Items["UserId"]);
        }
    }
}
