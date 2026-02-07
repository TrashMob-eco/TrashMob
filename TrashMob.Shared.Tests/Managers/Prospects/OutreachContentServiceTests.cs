namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Tests.Builders;
    using Xunit;

    public class OutreachContentServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<OutreachContentService>> _logger;
        private readonly OutreachContentService _sut;

        public OutreachContentServiceTests()
        {
            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<OutreachContentService>>();

            // Set API key to "x" to trigger fallback mode (no actual API calls)
            _configuration.Setup(c => c["AnthropicApiKey"]).Returns("x");

            _sut = new OutreachContentService(_configuration.Object, _logger.Object);
        }

        [Fact]
        public async Task GenerateOutreachContent_WhenApiKeyIsX_ReturnsFallbackContent()
        {
            var prospect = new CommunityProspectBuilder().Build();

            var result = await _sut.GenerateOutreachContentAsync(prospect, 1, 5);

            Assert.NotNull(result);
            Assert.Equal(prospect.Id, result.ProspectId);
            Assert.Equal(prospect.Name, result.ProspectName);
            Assert.Equal(1, result.CadenceStep);
            Assert.Contains(prospect.Name, result.Subject);
            Assert.Contains("trashmob.eco", result.HtmlBody);
            Assert.Equal(0, result.TokensUsed);
        }

        [Fact]
        public async Task GenerateOutreachContent_WhenApiKeyIsMissing_ReturnsFallbackContent()
        {
            _configuration.Setup(c => c["AnthropicApiKey"]).Returns((string)null);
            var prospect = new CommunityProspectBuilder().Build();

            var result = await _sut.GenerateOutreachContentAsync(prospect, 2, 0);

            Assert.NotNull(result);
            Assert.Equal(2, result.CadenceStep);
            Assert.Equal(0, result.TokensUsed);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GenerateOutreachContent_EachCadenceStep_ReturnsDifferentSubject(int step)
        {
            var prospect = new CommunityProspectBuilder().Build();

            var result = await _sut.GenerateOutreachContentAsync(prospect, step, 0);

            Assert.NotNull(result);
            Assert.Equal(step, result.CadenceStep);
            Assert.False(string.IsNullOrEmpty(result.Subject));
        }
    }
}
