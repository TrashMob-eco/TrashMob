namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
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
        private readonly ILogger<UserIsValidUserAuthHandler> logger;

        public UserIsPartnerUserOrIsAdminAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager, IBaseManager<PartnerAdmin> partnerUserManager, ILogger<UserIsValidUserAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.partnerUserManager = partnerUserManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsPartnerUserOrIsAdminRequirement requirement, Partner resource)
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while authenticating user.");
            }

        }
    }
}
