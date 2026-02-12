namespace TrashMob.Shared.Tests.Security
{
    using System;
    using System.Security.Claims;
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

    public class UserIsValidUserAuthHandlerTest
    {
        private readonly Mock<IUserManager> _mockUserManager;
        private readonly Mock<ILogger<UserIsValidUserAuthHandler>> _mockLogger;
        private readonly UserIsValidUserAuthHandler _sut;
        private readonly Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor> _mockHttpContextAccessor;

        public UserIsValidUserAuthHandlerTest()
        {
            _mockUserManager = new Mock<IUserManager>();
            _mockLogger = new Mock<ILogger<UserIsValidUserAuthHandler>>();
            _mockHttpContextAccessor = AuthHandlerTestHelper.CreateHttpContextAccessor();
            _sut = new UserIsValidUserAuthHandler(
                _mockHttpContextAccessor.Object,
                _mockUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_ExistingUser_Succeeds()
        {
            var user = new UserBuilder().WithEmail("joe@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("joe@test.com");
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_ExistingUser_SetsUserIdInHttpContext()
        {
            var userId = Guid.NewGuid();
            var user = new UserBuilder().WithId(userId).WithEmail("joe@test.com").Build();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("joe@test.com");
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.Equal(userId, _mockHttpContextAccessor.Object.HttpContext.Items["UserId"]);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_AutoCreatesUser()
        {
            var objectId = Guid.NewGuid();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.GetUserByUserNameAsync("newuser", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => u);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("email", "newuser@test.com"),
                new Claim("oid", objectId.ToString()));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
            _mockUserManager.Verify(m => m.AddAsync(
                It.Is<User>(u => u.Email == "newuser@test.com" && u.ObjectId == objectId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_NoOidClaim_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipal("newuser@test.com");
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
            _mockUserManager.Verify(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotFound_InvalidOid_DoesNotSucceed()
        {
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("email", "newuser@test.com"),
                new Claim("oid", "not-a-guid"));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
            _mockUserManager.Verify(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleRequirementAsync_AutoCreate_DuplicateUserName_AppendsOidSuffix()
        {
            var objectId = Guid.NewGuid();
            var existingUser = new UserBuilder().WithUserName("newuser").Build();

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.GetUserByUserNameAsync("newuser", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _mockUserManager.Setup(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => u);

            var expectedUserName = $"newuser_{objectId.ToString()[..8]}";

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("email", "newuser@test.com"),
                new Claim("oid", objectId.ToString()));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
            _mockUserManager.Verify(m => m.AddAsync(
                It.Is<User>(u => u.UserName == expectedUserName),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_AutoCreate_PopulatesNameAndPhoto()
        {
            var objectId = Guid.NewGuid();
            _mockUserManager.Setup(m => m.GetUserByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.GetUserByUserNameAsync("newuser", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => u);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("email", "newuser@test.com"),
                new Claim("oid", objectId.ToString()),
                new Claim("given_name", "Joe"),
                new Claim("family_name", "Smith"),
                new Claim("picture", "https://example.com/photo.jpg"));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            _mockUserManager.Verify(m => m.AddAsync(
                It.Is<User>(u =>
                    u.GivenName == "Joe" &&
                    u.Surname == "Smith" &&
                    u.ProfilePhotoUrl == "https://example.com/photo.jpg"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_NoEmailClaim_DoesNotSucceed()
        {
            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("sub", "some-subject"));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_SocialProfilePopulation_FillsEmptyFields()
        {
            var user = new UserBuilder()
                .WithEmail("joe@test.com")
                .Build();
            user.GivenName = null;
            user.Surname = null;
            user.ProfilePhotoUrl = null;

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("email", "joe@test.com"),
                new Claim("given_name", "Joe"),
                new Claim("family_name", "Smith"),
                new Claim("picture", "https://example.com/photo.jpg"));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
            _mockUserManager.Verify(m => m.UpdateAsync(
                It.Is<User>(u =>
                    u.GivenName == "Joe" &&
                    u.Surname == "Smith" &&
                    u.ProfilePhotoUrl == "https://example.com/photo.jpg"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleRequirementAsync_SocialProfilePopulation_DoesNotOverwriteExisting()
        {
            var user = new UserBuilder()
                .WithEmail("joe@test.com")
                .Build();
            user.GivenName = "ExistingName";
            user.Surname = "ExistingSurname";
            user.ProfilePhotoUrl = "https://existing.com/photo.jpg";

            _mockUserManager.Setup(m => m.GetUserByEmailAsync("joe@test.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var principal = AuthHandlerTestHelper.CreateClaimsPrincipalWithClaims(
                new Claim("email", "joe@test.com"),
                new Claim("given_name", "NewName"),
                new Claim("family_name", "NewSurname"),
                new Claim("picture", "https://new.com/photo.jpg"));
            var requirement = new UserIsValidUserRequirement();
            var context = AuthHandlerTestHelper.CreateAuthorizationHandlerContext(principal, requirement);

            await ((IAuthorizationHandler)_sut).HandleAsync(context);

            Assert.True(context.HasSucceeded);
            _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
