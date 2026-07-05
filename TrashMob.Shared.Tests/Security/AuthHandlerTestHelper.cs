namespace TrashMob.Shared.Tests.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;

    public static class AuthHandlerTestHelper
    {
        /// <summary>
        /// Creates a mock <see cref="IUserRoleService"/> whose <c>HasRoleAsync</c> defaults to
        /// <c>false</c>. Tests that need the caller to hold a role should call
        /// <see cref="GrantSiteAdmin"/> (or add their own <c>Setup</c> on the returned mock).
        /// </summary>
        public static Mock<IUserRoleService> CreateUserRoleService()
        {
            var mock = new Mock<IUserRoleService>();
            mock.Setup(s => s.HasRoleAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            return mock;
        }

        /// <summary>
        /// Configures the given <paramref name="mock"/> so that <c>HasRoleAsync</c> returns
        /// <c>true</c> when queried for the <c>SiteAdmin</c> role with the given
        /// <paramref name="userId"/>.
        /// </summary>
        public static void GrantSiteAdmin(this Mock<IUserRoleService> mock, Guid userId)
        {
            mock.Setup(s => s.HasRoleAsync(userId, RoleNames.SiteAdmin, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        public static ClaimsPrincipal CreateClaimsPrincipal(string email)
        {
            var claims = new List<Claim> { new("email", email) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        public static ClaimsPrincipal CreateClaimsPrincipalWithClaims(params Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        public static Mock<IHttpContextAccessor> CreateHttpContextAccessor()
        {
            var httpContext = new DefaultHttpContext();
            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(a => a.HttpContext).Returns(httpContext);
            return mockAccessor;
        }

        public static AuthorizationHandlerContext CreateAuthorizationHandlerContext<TRequirement>(
            ClaimsPrincipal principal, TRequirement requirement)
            where TRequirement : IAuthorizationRequirement
        {
            var requirements = new List<IAuthorizationRequirement> { requirement };
            return new AuthorizationHandlerContext(requirements, principal, null);
        }

        public static AuthorizationHandlerContext CreateAuthorizationHandlerContext<TRequirement>(
            ClaimsPrincipal principal, TRequirement requirement, object resource)
            where TRequirement : IAuthorizationRequirement
        {
            var requirements = new List<IAuthorizationRequirement> { requirement };
            return new AuthorizationHandlerContext(requirements, principal, resource);
        }
    }
}
