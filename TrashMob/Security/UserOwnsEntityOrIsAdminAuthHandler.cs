namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserOwnsEntityOrIsAdminAuthHandler : AuthorizationHandler<UserOwnsEntityOrIsAdminRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;

        public UserOwnsEntityOrIsAdminAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnsEntityOrIsAdminRequirement requirement, BaseModel resource)
        {
            var nameIdentifier = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await userManager.GetUserByNameIdentifierAsync(nameIdentifier, CancellationToken.None);

            if (user == null)
            {
                return;
            }

            httpContext.HttpContext.Items.Add("UserId", user.Id);

            if (user.Id == resource.CreatedByUserId || user.IsSiteAdmin)
            {
                context.Succeed(requirement);
            }
        }
    }
}
