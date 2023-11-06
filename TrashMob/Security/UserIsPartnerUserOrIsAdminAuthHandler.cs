﻿namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
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
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>() { new AuthorizationFailureReason(this, $"User with email '{email}' not found.") });
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
                        AuthorizationFailure.Failed(new List<AuthorizationFailureReason>() { new AuthorizationFailureReason(this, "User is not a partner user and is not a site admin.") });
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
