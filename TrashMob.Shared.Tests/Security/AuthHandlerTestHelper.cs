namespace TrashMob.Shared.Tests.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Moq;

    public static class AuthHandlerTestHelper
    {
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
