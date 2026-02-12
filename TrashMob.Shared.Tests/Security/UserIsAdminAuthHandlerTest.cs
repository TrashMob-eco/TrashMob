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

    public class UserIsAdminAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<ILogger<UserIsValidUserAuthHandler>> _mockLogger;
        private readonly UserIsAdminAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserIsAdminAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockLogger = new Mock<ILogger<UserIsValidUserAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserIsAdminAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdminUser_Succeeds()
        {
            var user = new UserBuilder().WithEmail("admin@test.com").AsSiteAdmin().Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("admin@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NonAdminUser_DoesNotSucceed()
        {
            var user = new UserBuilder().WithEmail("user@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("user@test.com");
            var requirement = new UserIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("missing@test.com");
            var requirement = new UserIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_ValidUser_SetsUserIdInHttpContext()
        {
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("admin@test.com").AsSiteAdmin().Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("admin@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.Equal(userId, _mockHttpContextAccessor.Object.HttpContext.Items["UserId"]);
        }
    }
}
