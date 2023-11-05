namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserOwnsEntityAuthHandler : AuthorizationHandler<UserOwnsEntityRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;
        private readonly ILogger<UserIsValidUserAuthHandler> logger;

        public UserOwnsEntityAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager, ILogger<UserIsValidUserAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserOwnsEntityRequirement requirement, BaseModel resource)
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
                    httpContext.HttpContext.Response.ContentType = "application/json";
                    await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    context.Fail();
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
                else
                {
                    var securityErrors = new SecurityErrors();
                    securityErrors.AddError("User does not own entity.");
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(securityErrors));
                    httpContext.HttpContext.Response.ContentType = "application/json";
                    await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    context.Fail();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user.");
            }
        }
    }
}
