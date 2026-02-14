namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Anthropic;
    using Anthropic.Models.Messages;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Discovers community prospects using the Anthropic Claude API.
    /// Supports both freeform custom queries and location-based discovery.
    /// </summary>
    public class ClaudeDiscoveryService(
        IConfiguration configuration,
        ILogger<ClaudeDiscoveryService> logger) : IClaudeDiscoveryService
    {
        private static readonly SemaphoreSlim RateLimiter = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private const string SystemPrompt = """
            You are a research assistant helping TrashMob.eco find potential community partners
            for environmental cleanup events.

            You must return ONLY a JSON array where each element has these fields:
            - "name": string (organization or city name)
            - "type": string (one of: Municipality, Nonprofit, HOA, CivicOrg, Other)
            - "estimatedPopulation": number or null (population served if applicable)
            - "website": string or null (website URL if known)
            - "contactSuggestion": string or null (e.g., "City Clerk", "Executive Director", or a specific name)
            - "rationale": string (1-2 sentences explaining why this is a good prospect)

            Return ONLY the JSON array, no markdown fences, no other text.
            """;

        /// <inheritdoc />
        public async Task<DiscoveryResult> DiscoverProspectsAsync(DiscoveryRequest request,
            CancellationToken cancellationToken = default)
        {
            var apiKey = configuration["AnthropicApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "x")
            {
                logger.LogWarning("Anthropic API key not configured. Returning empty discovery result.");
                return new DiscoveryResult { Message = "AI discovery is not configured. Set the AnthropicApiKey." };
            }

            var maxResults = Math.Clamp(request.MaxResults, 1, 25);
            var userPrompt = BuildUserPrompt(request, maxResults);

            await RateLimiter.WaitAsync(cancellationToken);
            try
            {
                var client = new AnthropicClient { ApiKey = apiKey };
                var maxTokens = int.TryParse(configuration["AnthropicMaxTokens"], out var mt) ? mt : 4096;

                var response = await client.Messages.Create(new MessageCreateParams
                {
                    Model = Model.ClaudeSonnet4_5_20250929,
                    MaxTokens = maxTokens,
                    System = SystemPrompt,
                    Messages = [new MessageParam { Role = "user", Content = userPrompt }],
                }, cancellationToken: cancellationToken);

                // Extract text from the first content block
                string responseText = "[]";
                if (response.Content?.Count > 0 && response.Content[0].TryPickText(out var textBlock))
                {
                    responseText = textBlock.Text;
                }

                var tokensUsed = (int)((response.Usage?.InputTokens ?? 0) + (response.Usage?.OutputTokens ?? 0));

                // Strip markdown fences if Claude includes them despite instructions
                responseText = responseText.Trim();
                if (responseText.StartsWith("```"))
                {
                    var firstNewline = responseText.IndexOf('\n');
                    if (firstNewline > 0)
                    {
                        responseText = responseText[(firstNewline + 1)..];
                    }

                    if (responseText.EndsWith("```"))
                    {
                        responseText = responseText[..^3].Trim();
                    }
                }

                var prospects = JsonSerializer.Deserialize<List<DiscoveredProspect>>(responseText, JsonOptions)
                    ?? [];

                logger.LogInformation("Claude discovery returned {Count} prospects using {Tokens} tokens",
                    prospects.Count, tokensUsed);

                return new DiscoveryResult
                {
                    Prospects = prospects,
                    TokensUsed = tokensUsed,
                };
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse Claude response as JSON");
                return new DiscoveryResult
                {
                    Message = "AI returned a response that could not be parsed. Try rephrasing your query.",
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calling Claude API for prospect discovery");
                return new DiscoveryResult
                {
                    Message = $"AI discovery failed: {ex.Message}",
                };
            }
            finally
            {
                RateLimiter.Release();
            }
        }

        private static string BuildUserPrompt(DiscoveryRequest request, int maxResults)
        {
            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                // Freeform mode: admin wrote a custom research query
                return $"""
                    {request.Prompt}

                    Return up to {maxResults} results as a JSON array.
                    """;
            }

            // Location-based mode: build a standard prompt from City/Region/Country
            var locationParts = new[] { request.City, request.Region, request.Country }
                .Where(s => !string.IsNullOrWhiteSpace(s));
            var locationStr = string.Join(", ", locationParts);

            if (string.IsNullOrWhiteSpace(locationStr))
            {
                locationStr = "the United States";
            }

            return $"""
                Identify up to {maxResults} organizations in or near {locationStr} that would be
                good community partners for environmental cleanup events.

                Look for: municipalities, city/county governments, environmental nonprofits,
                homeowner associations (HOAs), civic organizations, parks departments, and
                conservation groups.
                """;
        }
    }
}
