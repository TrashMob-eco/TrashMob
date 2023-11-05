namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
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
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                string email = emailAddressClaim == null ? emailClaim.Value : emailAddressClaim.Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user == null)
                {
                    var securityErrors = new SecurityErrors();
                    securityErrors.AddError("User not found.");
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(securityErrors));
                    httpContext.HttpContext.Response.StatusCode = 403;
                    httpContext.HttpContext.Response.ContentType = "application/json";
                    await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
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
                    else
                    {
                        var securityErrors = new SecurityErrors();
                        securityErrors.AddError("User is not a partner user and is not an admin.");
                        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(securityErrors));
                        httpContext.HttpContext.Response.StatusCode = 403;
                        httpContext.HttpContext.Response.ContentType = "application/json";
                        await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user.");
            }
        }
    }
}
