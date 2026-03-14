namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
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
    using TrashMob.Shared.Poco;
    using Xunit;

    public class AdminV2ControllerTests
    {
        private readonly Mock<IKeyedManager<PartnerRequest>> partnerRequestManager = new();
        private readonly Mock<IEmailManager> emailManager = new();
        private readonly Mock<ILogger<AdminV2Controller>> logger = new();
        private readonly AdminV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public AdminV2ControllerTests()
        {
            controller = new AdminV2Controller(
                partnerRequestManager.Object,
                emailManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new GenericIdentity("test", "Bearer"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task UpdatePartnerRequest_ReturnsOk()
        {
            var requestId = Guid.NewGuid();
            var partnerRequest = new PartnerRequest
            {
                Id = requestId,
                Name = "Test Partner",
                PartnerRequestStatusId = 2,
            };

            partnerRequestManager
                .Setup(m => m.UpdateAsync(It.IsAny<PartnerRequest>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partnerRequest);

            var dto = new PartnerRequestDto
            {
                Id = requestId,
                Name = "Test Partner",
                PartnerRequestStatusId = 2,
            };

            var result = await controller.UpdatePartnerRequest(testUserId, dto, CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetEmailTemplates_ReturnsOk()
        {
            var templates = new List<EmailTemplate>
            {
                new() { Name = "Welcome", Content = "<h1>Welcome</h1>" },
                new() { Name = "EventReminder", Content = "<h1>Reminder</h1>" },
            };

            emailManager
                .Setup(m => m.GetEmailTemplatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            var result = await controller.GetEmailTemplates(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<EmailTemplateDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }
    }
}
