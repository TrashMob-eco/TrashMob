namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
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

    public class NewslettersAdminV2ControllerTests
    {
        private readonly Mock<INewsletterManager> newsletterManager = new();
        private readonly Mock<ILookupRepository<NewsletterTemplate>> templateRepository = new();
        private readonly Mock<ILogger<NewslettersAdminV2Controller>> logger = new();
        private readonly NewslettersAdminV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public NewslettersAdminV2ControllerTests()
        {
            controller = new NewslettersAdminV2Controller(
                newsletterManager.Object,
                templateRepository.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity([], "Bearer"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        #region GetNewsletters

        [Fact]
        public async Task GetNewsletters_ReturnsOk_WithNewsletterDtos()
        {
            var newsletters = new List<Newsletter>
            {
                new() { Id = Guid.NewGuid(), Subject = "Test Newsletter", Status = NewsletterStatus.Draft },
            };

            newsletterManager
                .Setup(m => m.GetNewslettersAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletters);

            var result = await controller.GetNewsletters(null, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region GetNewsletter

        [Fact]
        public async Task GetNewsletter_ReturnsOk_WhenFound()
        {
            var newsletterId = Guid.NewGuid();
            var newsletter = new Newsletter { Id = newsletterId, Subject = "Test" };

            newsletterManager
                .Setup(m => m.GetAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletter);

            var result = await controller.GetNewsletter(newsletterId, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetNewsletter_ReturnsNotFound_WhenNull()
        {
            var newsletterId = Guid.NewGuid();

            newsletterManager
                .Setup(m => m.GetAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Newsletter)null);

            var result = await controller.GetNewsletter(newsletterId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region CreateNewsletter

        [Fact]
        public async Task CreateNewsletter_ReturnsCreated()
        {
            var created = new Newsletter { Id = Guid.NewGuid(), Subject = "Test", Status = NewsletterStatus.Draft };

            newsletterManager
                .Setup(m => m.AddAsync(It.IsAny<Newsletter>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var dto = new CreateNewsletterDto { Subject = "Test" };

            var result = await controller.CreateNewsletter(dto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateNewsletter_ReturnsBadRequest_WhenSubjectEmpty()
        {
            var dto = new CreateNewsletterDto { Subject = "" };

            var result = await controller.CreateNewsletter(dto, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region UpdateNewsletter

        [Fact]
        public async Task UpdateNewsletter_ReturnsOk_WhenDraft()
        {
            var newsletterId = Guid.NewGuid();
            var newsletter = new Newsletter { Id = newsletterId, Subject = "Original", Status = NewsletterStatus.Draft };

            newsletterManager
                .Setup(m => m.GetAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletter);

            newsletterManager
                .Setup(m => m.UpdateAsync(It.IsAny<Newsletter>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletter);

            var dto = new UpdateNewsletterDto { Subject = "Updated" };

            var result = await controller.UpdateNewsletter(newsletterId, dto, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateNewsletter_ReturnsBadRequest_WhenNotDraft()
        {
            var newsletterId = Guid.NewGuid();
            var newsletter = new Newsletter { Id = newsletterId, Subject = "Original", Status = "Sent" };

            newsletterManager
                .Setup(m => m.GetAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletter);

            var dto = new UpdateNewsletterDto { Subject = "Updated" };

            var result = await controller.UpdateNewsletter(newsletterId, dto, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region DeleteNewsletter

        [Fact]
        public async Task DeleteNewsletter_ReturnsNoContent_WhenDraft()
        {
            var newsletterId = Guid.NewGuid();
            var newsletter = new Newsletter { Id = newsletterId, Status = NewsletterStatus.Draft };

            newsletterManager
                .Setup(m => m.GetAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletter);

            newsletterManager
                .Setup(m => m.DeleteAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.DeleteNewsletter(newsletterId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteNewsletter_ReturnsBadRequest_WhenNotDraft()
        {
            var newsletterId = Guid.NewGuid();
            var newsletter = new Newsletter { Id = newsletterId, Status = "Sent" };

            newsletterManager
                .Setup(m => m.GetAsync(newsletterId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsletter);

            var result = await controller.DeleteNewsletter(newsletterId, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion
    }
}
