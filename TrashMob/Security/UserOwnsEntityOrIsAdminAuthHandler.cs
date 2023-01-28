namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserOwnsEntityOrIsAdminAuthHandler : AuthorizationHandler<UserOwnsEntityOrIsAdminRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;
        private readonly ILogger<UserIsValidUserAuthHandler> logger;

        public UserOwnsEntityOrIsAdminAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager, ILogger<UserIsValidUserAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnsEntityOrIsAdminRequirement requirement, BaseModel resource)
        {
            try
            {
                var email = context.User.FindFirst(ClaimTypes.Email).Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user == null)
                {
                    return;
                }

                if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
                {
                    httpContext.HttpContext.Items.Add("UserId", user.Id);
                }

                if (user.Id == resource.CreatedByUserId || user.IsSiteAdmin)
                {
                    context.Succeed(requirement);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while authenticating user.");
            }
        }
    }
}
