namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserIsValidUserAuthHandler : AuthorizationHandler<UserIsValidUserRequirement>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;

        public UserIsValidUserAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsValidUserRequirement requirement)
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

            context.Succeed(requirement);
        }
    }
}
