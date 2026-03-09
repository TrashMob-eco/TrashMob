namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco;
    using Xunit;

    public class AppVersionV2ControllerTests
    {
        [Fact]
        public void GetAppVersion_ReturnsOkWithVersionInfo()
        {
            var configValues = new Dictionary<string, string>
            {
                { "AppVersion:MinimumVersion", "2.0.0" },
                { "AppVersion:RecommendedVersion", "2.5.0" },
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var logger = new Mock<ILogger<AppVersionV2Controller>>();
            var controller = new AppVersionV2Controller(configuration, logger.Object);

            var result = controller.GetAppVersion();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var info = Assert.IsType<AppVersionInfo>(okResult.Value);
            Assert.Equal("2.0.0", info.MinimumVersion);
            Assert.Equal("2.5.0", info.RecommendedVersion);
        }

        [Fact]
        public void GetAppVersion_ReturnsFallbackWhenConfigMissing()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();

            var logger = new Mock<ILogger<AppVersionV2Controller>>();
            var controller = new AppVersionV2Controller(configuration, logger.Object);

            var result = controller.GetAppVersion();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var info = Assert.IsType<AppVersionInfo>(okResult.Value);
            Assert.Equal("1.0.0", info.MinimumVersion);
            Assert.Equal("1.0.0", info.RecommendedVersion);
        }
    }
}
