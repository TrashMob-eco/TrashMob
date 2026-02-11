namespace TrashMob.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserIsValidUserAuthHandler : AuthorizationHandler<UserIsValidUserRequirement>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserIsValidUserAuthHandler> logger;
        private readonly IUserManager userManager;

        public UserIsValidUserAuthHandler(IHttpContextAccessor httpContext, IUserManager userManager,
            ILogger<UserIsValidUserAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserIsValidUserRequirement requirement)
        {
            try
            {
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                var email = emailAddressClaim == null ? emailClaim?.Value : emailAddressClaim?.Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user == null)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, $"User with email '{email}' not found.") });
                    return;
                }

                // Auto-populate profile fields from social provider claims (one-time fill)
                var needsUpdate = false;

                if (string.IsNullOrEmpty(user.ProfilePhotoUrl))
                {
                    var pictureClaim = context.User.FindFirst("picture");
                    if (pictureClaim != null)
                    {
                        user.ProfilePhotoUrl = pictureClaim.Value;
                        needsUpdate = true;
                    }
                }

                if (string.IsNullOrEmpty(user.GivenName))
                {
                    var givenNameClaim = context.User.FindFirst(ClaimTypes.GivenName)
                                      ?? context.User.FindFirst("given_name");
                    if (givenNameClaim != null)
                    {
                        user.GivenName = givenNameClaim.Value;
                        needsUpdate = true;
                    }
                }

                if (string.IsNullOrEmpty(user.Surname))
                {
                    var surnameClaim = context.User.FindFirst(ClaimTypes.Surname)
                                    ?? context.User.FindFirst("family_name");
                    if (surnameClaim != null)
                    {
                        user.Surname = surnameClaim.Value;
                        needsUpdate = true;
                    }
                }

                if (needsUpdate)
                {
                    await userManager.UpdateAsync(user, CancellationToken.None);
                }

                if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
                {
                    httpContext.HttpContext.Items.Add("UserId", user.Id);
                }

                context.Succeed(requirement);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user. {0}",
                    JsonConvert.SerializeObject(context.User));
            }
        }
    }
}