namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserOwnsEntityAuthHandler : AuthorizationHandler<UserOwnsEntityRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserRepository userRepository;

        public UserOwnsEntityAuthHandler(IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            this.httpContext = httpContext;
            this.userRepository = userRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnsEntityRequirement requirement, BaseModel resource)
        {
            var userName = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await userRepository.GetUserByNameIdentifier(userName);

            if (user == null)
            {
                return;
            }

            httpContext.HttpContext.Items.Add("UserId", user.Id);

            if (user.Id == resource.CreatedByUserId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
