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
    using TrashMob.Shared.Managers.Interfaces;

    public class UserIsValidUserAuthHandler : AuthorizationHandler<UserIsValidUserRequirement>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserManager userManager;
        private readonly ILogger<UserIsValidUserAuthHandler> logger;

        public UserIsValidUserAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager, ILogger<UserIsValidUserAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsValidUserRequirement requirement)
        {
            try
            {
                var allClaims = context.User.Claims;

                StringBuilder stringBuilder = new StringBuilder();
                foreach(var claim in allClaims)
                {
                    stringBuilder.AppendFormat("Claim: '{0}' : '{1}'\n", claim.Subject.Name, claim.Value);                    
                }

                logger.LogInformation(stringBuilder.ToString());

                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                string email = emailAddressClaim == null ? emailClaim?.Value : emailAddressClaim?.Value;

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
            catch(Exception ex)
            {
                logger.LogError(ex, "Error occured while authenticating user. {0}", JsonConvert.SerializeObject(context.User));
            }
        }
    }
}
