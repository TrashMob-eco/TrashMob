namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using TrashMob.Shared.Managers.Prospects;
    using Xunit;

    public class SentimentAnalysisServiceTests
    {
        [Fact]
        public async Task AnalyzeSentiment_WhenApiKeyNotConfigured_ReturnsNeutral()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection([])
                .Build();

            var sut = new SentimentAnalysisService(config, NullLogger<SentimentAnalysisService>.Instance);

            var result = await sut.AnalyzeSentimentAsync("This is great!");

            Assert.Equal("Neutral", result);
        }

        [Fact]
        public async Task AnalyzeSentiment_WhenApiKeyIsX_ReturnsNeutral()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection([
                    new("anthropicApiKey", "x"),
                ])
                .Build();

            var sut = new SentimentAnalysisService(config, NullLogger<SentimentAnalysisService>.Instance);

            var result = await sut.AnalyzeSentimentAsync("This is great!");

            Assert.Equal("Neutral", result);
        }
    }
}
