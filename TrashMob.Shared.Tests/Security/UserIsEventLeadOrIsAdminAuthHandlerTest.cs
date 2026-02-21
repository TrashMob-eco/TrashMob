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

    public class UserIsEventLeadOrIsAdminAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<IEventAttendeeManager> _mockEventAttendeeManager;
        private readonly Mock<ILogger<UserIsEventLeadOrIsAdminAuthHandler>> _mockLogger;
        private readonly UserIsEventLeadOrIsAdminAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserIsEventLeadOrIsAdminAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockEventAttendeeManager = new Mock<IEventAttendeeManager>();
            _mockLogger = new Mock<ILogger<UserIsEventLeadOrIsAdminAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserIsEventLeadOrIsAdminAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockEventAttendeeManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdminUser_Succeeds()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("admin@test.com").AsSiteAdmin().Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("admin@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new Event { Id = Guid.NewGuid(), CreatedByUserId = otherUserId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsEventLeadOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
            // Admin takes early exit — no event lead check should be made
            _mockEventAttendeeManager.Verify(
                m => m.IsEventLeadAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleRequirementAsync_EventCreator_Succeeds()
        {
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("lead@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("lead@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new Event { Id = Guid.NewGuid(), CreatedByUserId = userId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("lead@test.com");
            var requirement = new UserIsEventLeadOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_CoLead_Succeeds()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("colead@test.com").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("colead@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventAttendeeManager.Setup(m => m.IsEventLeadAsync(eventId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resource = new Event { Id = eventId, CreatedByUserId = otherUserId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("colead@test.com");
            var requirement = new UserIsEventLeadOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NonLeadNonAdmin_DoesNotSucceed()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockEventAttendeeManager.Setup(m => m.IsEventLeadAsync(eventId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resource = new Event { Id = eventId, CreatedByUserId = otherUserId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("user@test.com");
            var requirement = new UserIsEventLeadOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var resource = new Event { Id = Guid.NewGuid(), CreatedByUserId = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("missing@test.com");
            var requirement = new UserIsEventLeadOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

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

            var resource = new Event { Id = Guid.NewGuid(), CreatedByUserId = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsEventLeadOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.Equal(userId, _mockHttpContextAccessor.Object.HttpContext.Items["UserId"]);
        }
    }
}
