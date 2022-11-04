namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserIsPartnerUserOrIsAdminAuthHandler : AuthorizationHandler<UserIsPartnerUserOrIsAdminRequirement, Partner>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;
        private readonly IBaseManager<PartnerAdmin> partnerUserManager;

        public UserIsPartnerUserOrIsAdminAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager, IBaseManager<PartnerAdmin> partnerUserManager)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.partnerUserManager = partnerUserManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsPartnerUserOrIsAdminRequirement requirement, Partner resource)
        {
            var userName = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = await userManager.GetUserByUserNameAsync(userName, CancellationToken.None);

            if (user == null)
            {
                return;
            }

            httpContext.HttpContext.Items.Add("UserId", user.Id);

            if (user.IsSiteAdmin)
            {
                context.Succeed(requirement);
            }
            else
            {
                var currentUserPartner = (await partnerUserManager.GetAsync(pu => pu.PartnerId == resource.Id && pu.UserId == user.Id, CancellationToken.None)).FirstOrDefault();

                if (currentUserPartner != null)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
