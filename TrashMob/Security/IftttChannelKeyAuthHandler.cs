namespace TrashMob.Security
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Text;
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
                    var securityErrors = new SecurityErrors();
                    securityErrors.AddError("IFTTT Channel Key Header not found.");
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(securityErrors));
                    httpContext.HttpContext.Response.StatusCode = 401;
                    httpContext.HttpContext.Response.ContentType = "application/json";
                    await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    return;
                }

                if (httpContext.HttpContext.Request.Headers.TryGetValue("IFTTT-Service-Key", out var iftttServiceKeyRequest))
                {
                    var securityErrors = new SecurityErrors();
                    securityErrors.AddError("IFTTT Service Key Header not found.");
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(securityErrors));
                    httpContext.HttpContext.Response.StatusCode = 401;
                    httpContext.HttpContext.Response.ContentType = "application/json";
                    await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    return;
                }

                var iftttChannelKeySecret = keyVaultManager.GetSecret("IftttChannelKey");
                var iftttServiceKeySecret = keyVaultManager.GetSecret("IftttServiceKey");

                if (iftttChannelKeyRequest != iftttChannelKeySecret ||
                    iftttServiceKeyRequest != iftttServiceKeySecret)
                {
                    var securityErrors = new SecurityErrors();
                    securityErrors.AddError("IFTTT Key Mismatch. Access Denied.");
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(securityErrors));
                    httpContext.HttpContext.Response.StatusCode = 401;
                    httpContext.HttpContext.Response.ContentType = "application/json";
                    await httpContext.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    return;
                }

                context.Succeed(requirement);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking IFTTT Service Keys.");
            }
        }
    }
}
