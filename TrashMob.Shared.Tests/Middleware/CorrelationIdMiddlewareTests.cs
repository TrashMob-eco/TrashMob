namespace TrashMob.Shared.Tests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Middleware;
    using Xunit;

    public class CorrelationIdMiddlewareTests
    {
        private readonly Mock<ILogger<CorrelationIdMiddleware>> logger = new();

        [Fact]
        public async Task InvokeAsync_GeneratesCorrelationId_WhenNotProvided()
        {
            var context = new DefaultHttpContext();
            var nextCalled = false;

            var middleware = new CorrelationIdMiddleware(_ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, logger.Object);

            await middleware.InvokeAsync(context);

            Assert.True(nextCalled);
            Assert.NotNull(context.Items["CorrelationId"]);
            Assert.True(Guid.TryParse(context.Items["CorrelationId"]?.ToString(), out _));
        }

        [Fact]
        public async Task InvokeAsync_UsesExistingCorrelationId_WhenProvided()
        {
            var context = new DefaultHttpContext();
            var existingId = "my-custom-correlation-id";
            context.Request.Headers["X-Correlation-ID"] = existingId;

            var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, logger.Object);

            await middleware.InvokeAsync(context);

            Assert.Equal(existingId, context.Items["CorrelationId"]);
        }

        [Fact]
        public async Task InvokeAsync_RegistersOnStartingCallback()
        {
            var context = new DefaultHttpContext();
            var existingId = "test-correlation-id";
            context.Request.Headers["X-Correlation-ID"] = existingId;

            string capturedCorrelationId = null;

            var middleware = new CorrelationIdMiddleware(ctx =>
            {
                // Capture the correlation ID that was stored by the middleware
                capturedCorrelationId = ctx.Items["CorrelationId"]?.ToString();
                return Task.CompletedTask;
            }, logger.Object);

            await middleware.InvokeAsync(context);

            // The correlation ID should be stored in Items for the response callback to use
            Assert.Equal(existingId, capturedCorrelationId);
            Assert.Equal(existingId, context.Items["CorrelationId"]);
        }
    }
}
