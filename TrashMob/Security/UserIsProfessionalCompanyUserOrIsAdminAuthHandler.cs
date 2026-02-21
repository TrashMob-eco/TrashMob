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
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class
        UserIsProfessionalCompanyUserOrIsAdminAuthHandler : AuthorizationHandler<UserIsProfessionalCompanyUserOrIsAdminRequirement, ProfessionalCompany>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserIsProfessionalCompanyUserOrIsAdminAuthHandler> logger;
        private readonly IProfessionalCompanyUserManager companyUserManager;
        private readonly IUserManager userManager;

        public UserIsProfessionalCompanyUserOrIsAdminAuthHandler(
            IHttpContextAccessor httpContext,
            IUserManager userManager,
            IProfessionalCompanyUserManager companyUserManager,
            ILogger<UserIsProfessionalCompanyUserOrIsAdminAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.companyUserManager = companyUserManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserIsProfessionalCompanyUserOrIsAdminRequirement requirement,
            ProfessionalCompany resource)
        {
            try
            {
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                var email = emailAddressClaim is null ? emailClaim?.Value : emailAddressClaim?.Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user is null)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, $"User with email '{email}' not found.") });
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
                    var isCompanyUser = await companyUserManager.IsCompanyUserAsync(
                        resource.Id, user.Id, CancellationToken.None);

                    if (isCompanyUser)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                            { new(this, "User is not a professional company user and is not a site admin.") });
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
