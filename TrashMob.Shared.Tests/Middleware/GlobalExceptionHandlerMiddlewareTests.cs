namespace TrashMob.Shared.Tests.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Middleware;
    using Xunit;

    public class GlobalExceptionHandlerMiddlewareTests
    {
        private static readonly JsonSerializerOptions CamelCaseOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> logger = new();

        private GlobalExceptionHandlerMiddleware CreateMiddleware(
            RequestDelegate next,
            bool isDevelopment = true)
        {
            var env = new Mock<IHostEnvironment>();
            env.Setup(e => e.EnvironmentName).Returns(isDevelopment ? "Development" : "Production");
            return new GlobalExceptionHandlerMiddleware(next, logger.Object, env.Object);
        }

        private static async Task<ProblemDetails> GetProblemDetailsFromResponse(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return await JsonSerializer.DeserializeAsync<ProblemDetails>(
                context.Response.Body, CamelCaseOptions);
        }

        [Fact]
        public async Task InvokeAsync_Returns400_ForArgumentException()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            context.Items["CorrelationId"] = "test-id";

            var middleware = CreateMiddleware(_ => throw new ArgumentException("Bad argument"));

            await middleware.InvokeAsync(context);

            Assert.Equal(400, context.Response.StatusCode);
            var problem = await GetProblemDetailsFromResponse(context);
            Assert.Equal("Bad Request", problem.Title);
        }

        [Fact]
        public async Task InvokeAsync_Returns404_ForKeyNotFoundException()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            var middleware = CreateMiddleware(_ => throw new KeyNotFoundException("Not found"));

            await middleware.InvokeAsync(context);

            Assert.Equal(404, context.Response.StatusCode);
            var problem = await GetProblemDetailsFromResponse(context);
            Assert.Equal("Not Found", problem.Title);
        }

        [Fact]
        public async Task InvokeAsync_Returns401_ForUnauthorizedAccessException()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            var middleware = CreateMiddleware(_ => throw new UnauthorizedAccessException());

            await middleware.InvokeAsync(context);

            Assert.Equal(401, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_Returns500_ForUnknownException()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            var middleware = CreateMiddleware(_ => throw new Exception("Something broke"));

            await middleware.InvokeAsync(context);

            Assert.Equal(500, context.Response.StatusCode);
            var problem = await GetProblemDetailsFromResponse(context);
            Assert.Equal("Internal Server Error", problem.Title);
        }

        [Fact]
        public async Task InvokeAsync_IncludesCorrelationId_WhenPresent()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            context.Items["CorrelationId"] = "my-correlation-id";

            var middleware = CreateMiddleware(_ => throw new Exception("fail"));

            await middleware.InvokeAsync(context);

            var problem = await GetProblemDetailsFromResponse(context);
            Assert.True(problem.Extensions.ContainsKey("correlationId"));
        }

        [Fact]
        public async Task InvokeAsync_IncludesDetail_InDevelopment()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            var middleware = CreateMiddleware(_ => throw new Exception("Detailed error message"), isDevelopment: true);

            await middleware.InvokeAsync(context);

            var problem = await GetProblemDetailsFromResponse(context);
            Assert.Equal("Detailed error message", problem.Detail);
        }

        [Fact]
        public async Task InvokeAsync_HidesDetail_InProduction()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            var middleware = CreateMiddleware(_ => throw new Exception("Sensitive info"), isDevelopment: false);

            await middleware.InvokeAsync(context);

            var problem = await GetProblemDetailsFromResponse(context);
            Assert.Null(problem.Detail);
        }

        [Fact]
        public async Task InvokeAsync_SetsContentType_ToProblemJson()
        {
            var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
            var middleware = CreateMiddleware(_ => throw new Exception("fail"));

            await middleware.InvokeAsync(context);

            Assert.Equal("application/problem+json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_DoesNothing_WhenNoException()
        {
            var context = new DefaultHttpContext();
            var middleware = CreateMiddleware(_ => Task.CompletedTask);

            await middleware.InvokeAsync(context);

            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}
