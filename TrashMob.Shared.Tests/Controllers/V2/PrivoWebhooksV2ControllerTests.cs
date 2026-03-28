namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class PrivoWebhooksV2ControllerTests
    {
        private readonly Mock<IPrivoConsentManager> privoConsentManager = new();
        private readonly Mock<ILogger<PrivoWebhooksV2Controller>> logger = new();
        private readonly PrivoWebhooksV2Controller controller;

        public PrivoWebhooksV2ControllerTests()
        {
            controller = new PrivoWebhooksV2Controller(privoConsentManager.Object, logger.Object);
        }

        [Fact]
        public async Task ProcessConsentEvent_ReturnsOk()
        {
            var payload = new PrivoWebhookPayload
            {
                Id = "webhook-123",
                Sid = "sid-456",
                EventTypes = ["consent_request_created"],
                ConsentIdentifiers = ["consent-789"],
            };

            var result = await controller.ProcessConsentEvent(payload);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ProcessConsentEvent_ReturnsOk_EvenOnProcessingError()
        {
            privoConsentManager
                .Setup(m => m.ProcessWebhookAsync(It.IsAny<PrivoWebhookPayload>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Test error"));

            var payload = new PrivoWebhookPayload
            {
                Id = "webhook-err",
                Sid = "sid-456",
                EventTypes = ["consent_request_created"],
                ConsentIdentifiers = ["consent-789"],
            };

            var result = await controller.ProcessConsentEvent(payload);

            // Always returns 200 to PRIVO even on internal error
            Assert.IsType<OkResult>(result);
        }
    }
}
