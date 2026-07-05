namespace TrashMob.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Authorization handler for the <see cref="AuthorizationPolicyConstants.UserIsSalesRepOrIsAdmin"/>
    /// policy. Succeeds when the calling user is granted the
    /// <see cref="RoleNames.SalesRep"/> or <see cref="RoleNames.SiteAdmin"/> role.
    /// </summary>
    public class UserIsSalesRepOrIsAdminAuthHandler : AuthorizationHandler<UserIsSalesRepOrIsAdminRequirement>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserIsSalesRepOrIsAdminAuthHandler> logger;
        private readonly IUserManager userManager;
        private readonly IUserRoleService userRoleService;

        public UserIsSalesRepOrIsAdminAuthHandler(
            IHttpContextAccessor httpContext,
            IUserManager userManager,
            IUserRoleService userRoleService,
            ILogger<UserIsSalesRepOrIsAdminAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.userRoleService = userRoleService;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserIsSalesRepOrIsAdminRequirement requirement)
        {
            try
            {
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                var email = emailAddressClaim is null ? emailClaim?.Value : emailAddressClaim?.Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user is null)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, $"User with email '{email}' not found.") });
                    return;
                }

                if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
                {
                    httpContext.HttpContext.Items.Add("UserId", user.Id);
                }

                if (await userRoleService.HasRoleAsync(user.Id, RoleNames.SiteAdmin, CancellationToken.None)
                    || await userRoleService.HasRoleAsync(user.Id, RoleNames.SalesRep, CancellationToken.None))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, "User is not a sales rep and is not a site admin.") });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user.");
            }
        }
    }
}
