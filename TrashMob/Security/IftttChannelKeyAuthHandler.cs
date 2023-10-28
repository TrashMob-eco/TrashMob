namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;

    public class IftttChannelKeyAuthHandler : AuthorizationHandler<IftttChannelKeyRequirement>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IKeyVaultManager keyVaultManager;
        private readonly ILogger<UserIsValidUserAuthHandler> logger;

        public IftttChannelKeyAuthHandler(IHttpContextAccessor httpContext, IKeyVaultManager keyVaultManager, ILogger<UserIsValidUserAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.keyVaultManager = keyVaultManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IftttChannelKeyRequirement requirement)
        {
            try
            {
                if (httpContext.HttpContext.Request.Headers.TryGetValue("IFTTT-Channel-Key", out var iftttChannelKeyRequest))
                {
                    return;
                }

                if (httpContext.HttpContext.Request.Headers.TryGetValue("IFTTT-Service-Key", out var iftttServiceKeyRequest))
                {
                    return;
                }

                var iftttChannelKeySecret = keyVaultManager.GetSecret("IftttChannelKey");
                var iftttServiceKeySecret = keyVaultManager.GetSecret("IftttServiceKey");

                if (iftttChannelKeyRequest != iftttChannelKeySecret ||
                    iftttServiceKeyRequest != iftttServiceKeySecret)
                {
                    return;
                }
                
                context.Succeed(requirement);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while checking Ifttt Service Keys.");
            }
        }
    }
}
