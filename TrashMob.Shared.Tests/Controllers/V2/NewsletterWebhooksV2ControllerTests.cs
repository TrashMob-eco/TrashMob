namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers;
    using TrashMob.Controllers.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class NewsletterWebhooksV2ControllerTests
    {
        private readonly Mock<INewsletterManager> newsletterManager = new();
        private readonly Mock<ILogger<NewsletterWebhooksV2Controller>> logger = new();
        private readonly NewsletterWebhooksV2Controller controller;

        public NewsletterWebhooksV2ControllerTests()
        {
            controller = new NewsletterWebhooksV2Controller(
                newsletterManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        #region ProcessNewsletterEvents

        [Fact]
        public async Task ProcessNewsletterEvents_ReturnsOk_WhenEventsProcessed()
        {
            var newsletterId = Guid.NewGuid();
            var events = new List<SendGridEvent>
            {
                new() { Event = "delivered", NewsletterId = newsletterId.ToString() },
                new() { Event = "open", NewsletterId = newsletterId.ToString() },
            };

            newsletterManager
                .Setup(m => m.UpdateStatisticsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.ProcessNewsletterEvents(events, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ProcessNewsletterEvents_ReturnsOk_WhenEventsNull()
        {
            var result = await controller.ProcessNewsletterEvents(null, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ProcessNewsletterEvents_ReturnsOk_WhenEventsFail()
        {
            var newsletterId = Guid.NewGuid();
            var events = new List<SendGridEvent>
            {
                new() { Event = "delivered", NewsletterId = newsletterId.ToString() },
            };

            newsletterManager
                .Setup(m => m.UpdateStatisticsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("SendGrid error"));

            var result = await controller.ProcessNewsletterEvents(events, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }

        #endregion
    }
}
