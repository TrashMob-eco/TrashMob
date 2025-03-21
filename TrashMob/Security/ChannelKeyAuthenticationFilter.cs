﻿namespace TrashMob.Security
{
    using System;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;

    public class ChannelKeyAuthenticationFilter : IAsyncAuthorizationFilter
    {
        private readonly IKeyVaultManager keyVaultManager;
        private readonly ILogger<ChannelKeyAuthenticationFilter> logger;

        public ChannelKeyAuthenticationFilter(IKeyVaultManager keyVaultManager,
            ILogger<ChannelKeyAuthenticationFilter> logger)
        {
            Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            this.keyVaultManager = keyVaultManager;
            this.logger = logger;
        }

        public AuthorizationPolicy Policy { get; }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                if (!context.HttpContext.Request.Headers.TryGetValue("IFTTT-Channel-Key",
                        out var iftttChannelKeyRequest))
                {
                    // Return custom 401 result
                    context.Result = new JsonResult(new
                    {
                        errors = new JsonArray
                        {
                            new
                            {
                                error = "IFTTT Channel Key Header not found.",
                            },
                        },
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };

                    return Task.CompletedTask;
                }

                if (!context.HttpContext.Request.Headers.TryGetValue("IFTTT-Service-Key",
                        out var iftttServiceKeyRequest))
                {
                    // Return custom 401 result
                    context.Result = new JsonResult(new
                    {
                        errors = new JsonArray
                        {
                            new
                            {
                                error = "IFTTT Service Key Header not found.",
                            },
                        },
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };

                    return Task.CompletedTask;
                }

                var iftttChannelKeySecret = keyVaultManager.GetSecret("IFTTT-Channel-Key");
                var iftttServiceKeySecret = keyVaultManager.GetSecret("IFTTT-Service-Key");

                if (iftttChannelKeyRequest != iftttChannelKeySecret ||
                    iftttServiceKeyRequest != iftttServiceKeySecret)
                {
                    context.Result = new JsonResult(new
                    {
                        errors = new JsonArray
                        {
                            new
                            {
                                error = "IFTTT Key Mismatch. Access Denied.",
                            },
                        },
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };

                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking IFTTT Service Keys.");
                context.Result = new JsonResult(new
                {
                    errors = new JsonArray
                    {
                        new
                        {
                            error = "A server error has occurred.",
                        },
                    },
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }

            return Task.CompletedTask;
        }
    }
}