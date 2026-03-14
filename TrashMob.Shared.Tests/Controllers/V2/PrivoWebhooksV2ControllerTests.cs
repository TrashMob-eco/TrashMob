namespace TrashMob.Shared.Tests.Controllers.V2
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco;
    using Xunit;

    public class PrivoWebhooksV2ControllerTests
    {
        private readonly Mock<ILogger<PrivoWebhooksV2Controller>> logger = new();
        private readonly PrivoWebhooksV2Controller controller;

        public PrivoWebhooksV2ControllerTests()
        {
            controller = new PrivoWebhooksV2Controller(logger.Object);
        }

        [Fact]
        public void ProcessConsentEvent_ReturnsOk()
        {
            var consentEvent = new PrivoConsentEvent
            {
                EventType = "consent_granted",
                ConsentRequestId = "req-123",
                Status = "granted",
            };

            var result = controller.ProcessConsentEvent(consentEvent);

            Assert.IsType<OkResult>(result);
        }
    }
}
