namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserOwnsEntityOrIsAdminAuthHandler : AuthorizationHandler<UserOwnsEntityOrIsAdminRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserRepository userRepository;

        public UserOwnsEntityOrIsAdminAuthHandler(IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            this.httpContext = httpContext;
            this.userRepository = userRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnsEntityOrIsAdminRequirement requirement, BaseModel resource)
        {
            var userName = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await userRepository.GetUserByNameIdentifier(userName);

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
