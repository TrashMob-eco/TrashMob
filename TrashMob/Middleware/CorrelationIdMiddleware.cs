namespace TrashMob.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public async Task InvokeAsync(HttpContext context)
        {
            var headerValue = context.Request.Headers[CorrelationIdHeader].ToString();
            var correlationId = string.IsNullOrEmpty(headerValue)
                                ? Guid.NewGuid().ToString()
                                : headerValue;

            context.Items["CorrelationId"] = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
                return Task.CompletedTask;
            });

            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                await next(context);
            }
        }
    }
}
