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

    public class UserIsProfessionalCompanyUserOrIsAdminAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<IProfessionalCompanyUserManager> _mockCompanyUserManager;
        private readonly Mock<ILogger<UserIsProfessionalCompanyUserOrIsAdminAuthHandler>> _mockLogger;
        private readonly UserIsProfessionalCompanyUserOrIsAdminAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserIsProfessionalCompanyUserOrIsAdminAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockCompanyUserManager = new Mock<IProfessionalCompanyUserManager>();
            _mockLogger = new Mock<ILogger<UserIsProfessionalCompanyUserOrIsAdminAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserIsProfessionalCompanyUserOrIsAdminAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockCompanyUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdminUser_Succeeds()
        {
            var user = new UserBuilder().WithEmail("admin@test.com").AsSiteAdmin().Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("admin@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var resource = new ProfessionalCompany { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsProfessionalCompanyUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_CompanyUser_Succeeds()
        {
            var userId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("company@test.com").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("company@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockCompanyUserManager.Setup(m => m.IsCompanyUserAsync(companyId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resource = new ProfessionalCompany { Id = companyId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("company@test.com");
            var requirement = new UserIsProfessionalCompanyUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NonCompanyNonAdmin_DoesNotSucceed()
        {
            var userId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockCompanyUserManager.Setup(m => m.IsCompanyUserAsync(companyId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resource = new ProfessionalCompany { Id = companyId };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("user@test.com");
            var requirement = new UserIsProfessionalCompanyUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("missing@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var resource = new ProfessionalCompany { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("missing@test.com");
            var requirement = new UserIsProfessionalCompanyUserOrIsAdminRequirement();
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

            var resource = new ProfessionalCompany { Id = Guid.NewGuid() };
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("admin@test.com");
            var requirement = new UserIsProfessionalCompanyUserOrIsAdminRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement, resource);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.Equal(userId, _mockHttpContextAccessor.Object.HttpContext.Items["UserId"]);
        }
    }
}
