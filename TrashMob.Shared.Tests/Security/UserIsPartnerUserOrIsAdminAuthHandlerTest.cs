namespace TrashMob.Shared.Tests.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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

    public class UserIsPartnerUserOrIsAdminAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<IBaseManager<PartnerAdmin>> _mockPartnerUserManager;
        private readonly Mock<ILogger<UserIsValidUserAuthHandler>> _mockLogger;
        private readonly UserIsPartnerUserOrIsAdminAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserIsPartnerUserOrIsAdminAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockPartnerUserManager = new Mock<IBaseManager<PartnerAdmin>>();
            _mockLogger = new Mock<ILogger<UserIsValidUserAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserIsPartnerUserOrIsAdminAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockPartnerUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdminUser_Succeeds()
        {
            var user = new UserBuilder().WithEmail("admin@test.com").AsSiteAdmin().Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("admin@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new Partner { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsPartnerUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_PartnerAdmin_Succeeds()
        {
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("partner@test.com").Build();
            var partnerAdmin = new PartnerAdmin { PartnerId = partnerId, UserId = userId };

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("partner@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPartnerUserManager.Setup(m => m.GetAsync(
                    It.IsAny<Expression<Func<PartnerAdmin, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PartnerAdmin> { partnerAdmin }.AsEnumerable());

            var resource = new Partner { Id = partnerId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("partner@test.com");
            var requirement = new UserIsPartnerUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NonPartnerNonAdmin_DoesNotSucceed()
        {
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockPartnerUserManager.Setup(m => m.GetAsync(
                    It.IsAny<Expression<Func<PartnerAdmin, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<PartnerAdmin>());

            var resource = new Partner { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("user@test.com");
            var requirement = new UserIsPartnerUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var resource = new Partner { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("missing@test.com");
            var requirement = new UserIsPartnerUserOrIsAdminRequirement();
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

            var resource = new Partner { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsPartnerUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.Equal(userId, _mockHttpContextAccessor.Object.HttpContext.Items["UserId"]);
        }
    }
}
