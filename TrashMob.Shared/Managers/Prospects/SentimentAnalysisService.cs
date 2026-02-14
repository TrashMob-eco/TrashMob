namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Anthropic;
    using Anthropic.Models.Messages;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Analyzes sentiment of prospect replies using the Anthropic Claude API.
    /// Falls back to "Neutral" when the API is not configured.
    /// </summary>
    public class SentimentAnalysisService(
        IConfiguration configuration,
        ILogger<SentimentAnalysisService> logger) : ISentimentAnalysisService
    {
        private static readonly SemaphoreSlim RateLimiter = new(1, 1);

        private const string SystemPrompt =
            "You are a sentiment classifier. Analyze the following text from a community partner prospect's reply to an outreach email about TrashMob.eco. " +
            "Respond with EXACTLY one word: \"Positive\", \"Neutral\", or \"Negative\". No other text.";

        private static readonly string[] ValidSentiments = ["Positive", "Neutral", "Negative"];

        public async Task<string> AnalyzeSentimentAsync(string replyText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(replyText))
            {
                return "Neutral";
            }

            var apiKey = configuration["AnthropicApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "x")
            {
                logger.LogDebug("Anthropic API key not configured. Returning Neutral sentiment.");
                return "Neutral";
            }

            await RateLimiter.WaitAsync(cancellationToken);
            try
            {
                var client = new AnthropicClient { ApiKey = apiKey };
                var response = await client.Messages.Create(new MessageCreateParams
                {
                    Model = "claude-3-5-haiku-20241022",
                    MaxTokens = 10,
                    System = SystemPrompt,
                    Messages = [new MessageParam { Role = "user", Content = replyText }],
                }, cancellationToken: cancellationToken);

                var result = "Neutral";
                if (response.Content?.Count > 0 && response.Content[0].TryPickText(out var textBlock))
                {
                    result = textBlock.Text?.Trim() ?? "Neutral";
                }

                // Validate response is one of the expected values
                foreach (var valid in ValidSentiments)
                {
                    if (result.Equals(valid, StringComparison.OrdinalIgnoreCase))
                    {
                        return valid;
                    }
                }

                logger.LogWarning("Unexpected sentiment response: {Response}. Defaulting to Neutral.", result);
                return "Neutral";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error analyzing sentiment. Defaulting to Neutral.");
                return "Neutral";
            }
            finally
            {
                RateLimiter.Release();
            }
        }
    }
}
