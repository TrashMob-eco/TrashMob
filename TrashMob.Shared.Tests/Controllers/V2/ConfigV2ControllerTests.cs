namespace TrashMob.Shared.Tests.Controllers.V2
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using Xunit;

    public class ConfigV2ControllerTests
    {
        private readonly Mock<IConfiguration> configuration = new();
        private readonly Mock<ILogger<ConfigV2Controller>> logger = new();
        private readonly ConfigV2Controller controller;

        public ConfigV2ControllerTests()
        {
            controller = new ConfigV2Controller(
                configuration.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        #region GetConfig

        [Fact]
        public void GetConfig_ReturnsOk_WithConfigData()
        {
            configuration
                .Setup(c => c["ApplicationInsights:ConnectionString"])
                .Returns("InstrumentationKey=test-key;IngestionEndpoint=https://test");

            configuration
                .Setup(c => c["AzureAdEntra:Instance"])
                .Returns("https://test.ciamlogin.com");

            configuration
                .Setup(c => c["AzureAdEntra:Domain"])
                .Returns("test.domain");

            configuration
                .Setup(c => c["AzureAdEntra:FrontendClientId"])
                .Returns("client-id");

            configuration
                .Setup(c => c["AzureAdEntra:TenantId"])
                .Returns("tenant-id");

            var result = controller.GetConfig();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetConfig_ReturnsOk_WhenConfigMissing()
        {
            configuration
                .Setup(c => c["ApplicationInsights:ConnectionString"])
                .Returns((string)null);

            configuration
                .Setup(c => c["AzureAdEntra:Instance"])
                .Returns((string)null);

            configuration
                .Setup(c => c["AzureAdEntra:Domain"])
                .Returns((string)null);

            configuration
                .Setup(c => c["AzureAdEntra:FrontendClientId"])
                .Returns((string)null);

            configuration
                .Setup(c => c["AzureAdEntra:TenantId"])
                .Returns((string)null);

            var result = controller.GetConfig();

            Assert.IsType<OkObjectResult>(result);
        }

        #endregion
    }
}
