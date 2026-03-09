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
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using Xunit;

    public class NewsletterPreferencesV2ControllerTests
    {
        private readonly Mock<IUserNewsletterPreferenceManager> preferenceManager = new();
        private readonly Mock<ILookupRepository<NewsletterCategory>> categoryRepository = new();
        private readonly Mock<ILogger<NewsletterPreferencesV2Controller>> logger = new();
        private readonly NewsletterPreferencesV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public NewsletterPreferencesV2ControllerTests()
        {
            controller = new NewsletterPreferencesV2Controller(
                preferenceManager.Object, categoryRepository.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task UnsubscribeAll_ReturnsNoContent()
        {
            var result = await controller.UnsubscribeAll(CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            preferenceManager.Verify(m => m.UnsubscribeAllAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
