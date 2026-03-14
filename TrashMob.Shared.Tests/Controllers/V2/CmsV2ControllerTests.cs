namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using TrashMob.Controllers.V2;
    using Xunit;

    public class CmsV2ControllerTests
    {
        private readonly Mock<IHttpClientFactory> mockFactory = new();
        private readonly Mock<IConfiguration> mockConfig = new();
        private readonly Mock<ILogger<CmsV2Controller>> logger = new();
        private readonly CmsV2Controller controller;

        public CmsV2ControllerTests()
        {
            mockConfig.Setup(c => c["StrapiBaseUrl"]).Returns("https://cms.test.com");

            controller = new CmsV2Controller(mockFactory.Object, mockConfig.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        private void SetupHttpClient(string responseContent)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json"),
                });

            var httpClient = new HttpClient(handler.Object);
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [Fact]
        public void GetAdminUrl_ReturnsOk()
        {
            var result = controller.GetAdminUrl();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetHeroSection_ReturnsStrapiContent()
        {
            SetupHttpClient("{\"data\":{\"title\":\"Welcome\"}}");

            var result = await controller.GetHeroSection(CancellationToken.None);

            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", contentResult.ContentType);
            Assert.NotNull(contentResult.Content);
        }

        [Fact]
        public async Task GetNewsCategories_ReturnsStrapiContent()
        {
            SetupHttpClient("{\"data\":[{\"id\":1,\"name\":\"Environment\"}]}");

            var result = await controller.GetNewsCategories(CancellationToken.None);

            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", contentResult.ContentType);
            Assert.NotNull(contentResult.Content);
        }
    }
}
