namespace TrashMob.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private static readonly Dictionary<Type, (HttpStatusCode Status, string Title)> ExceptionMap = new()
        {
            [typeof(ArgumentException)] = (HttpStatusCode.BadRequest, "Bad Request"),
            [typeof(ArgumentNullException)] = (HttpStatusCode.BadRequest, "Bad Request"),
            [typeof(KeyNotFoundException)] = (HttpStatusCode.NotFound, "Not Found"),
            [typeof(UnauthorizedAccessException)] = (HttpStatusCode.Unauthorized, "Unauthorized"),
            [typeof(InvalidOperationException)] = (HttpStatusCode.Conflict, "Conflict"),
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            var traceId = context.TraceIdentifier;

            logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}, TraceId: {TraceId}",
                correlationId, traceId);

            var (statusCode, title) = ExceptionMap.TryGetValue(exception.GetType(), out var mapped)
                ? mapped
                : (HttpStatusCode.InternalServerError, "Internal Server Error");

            var statusCodeInt = (int)statusCode;

            var problemDetails = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{statusCodeInt}",
                Status = statusCodeInt,
                Title = title,
                Detail = environment.IsDevelopment() ? exception.Message : null,
                Instance = context.Request.Path,
            };

            problemDetails.Extensions["traceId"] = traceId;

            if (correlationId is not null)
            {
                problemDetails.Extensions["correlationId"] = correlationId;
            }

            context.Response.StatusCode = statusCodeInt;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
        }
    }
}
