namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserOwnsEntityAuthHandler : AuthorizationHandler<UserOwnsEntityRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;

        public UserOwnsEntityAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnsEntityRequirement requirement, BaseModel resource)
        {
            var nameIdentifier = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await userManager.GetUserByNameIdentifierAsync(nameIdentifier, CancellationToken.None);

            if (user == null)
            {
                return;
            }

            if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
            {
                httpContext.HttpContext.Items.Add("UserId", user.Id);
            }

            if (user.Id == resource.CreatedByUserId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
