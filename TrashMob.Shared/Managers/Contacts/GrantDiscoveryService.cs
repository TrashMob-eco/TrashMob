namespace TrashMob.Shared.Managers.Contacts
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
    /// Discovers grant opportunities using the Anthropic Claude API.
    /// </summary>
    public class GrantDiscoveryService(
        IConfiguration configuration,
        ILogger<GrantDiscoveryService> logger) : IGrantDiscoveryService
    {
        private static readonly SemaphoreSlim RateLimiter = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private const string SystemPrompt = """
            You are a research assistant helping TrashMob.eco find grant funding opportunities.
            TrashMob.eco is a US-based 501(c)(3) nonprofit that organizes community environmental
            cleanup events (litter pickup, park restoration, waterway cleanups).

            You must return ONLY a JSON array where each element has these fields:
            - "funderName": string (foundation, agency, or corporate funder name)
            - "programName": string or null (specific grant program name)
            - "description": string (1-2 sentences about the grant purpose and requirements)
            - "amountMin": number or null (minimum award amount in USD)
            - "amountMax": number or null (maximum award amount in USD)
            - "deadline": string or null (submission deadline if known, ISO date format)
            - "url": string or null (link to grant information page)
            - "eligibilityNotes": string (why TrashMob qualifies or key eligibility criteria)
            - "rationale": string (why this grant is a good fit for TrashMob's mission)

            Return ONLY the JSON array, no markdown fences, no other text.
            """;

        /// <inheritdoc />
        public async Task<GrantDiscoveryResult> DiscoverGrantsAsync(GrantDiscoveryRequest request,
            CancellationToken cancellationToken = default)
        {
            var apiKey = configuration["AnthropicApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "x")
            {
                logger.LogWarning("Anthropic API key not configured. Returning empty grant discovery result.");
                return new GrantDiscoveryResult { Message = "AI discovery is not configured. Set the AnthropicApiKey." };
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

                var grants = JsonSerializer.Deserialize<List<DiscoveredGrant>>(responseText, JsonOptions)
                    ?? [];

                logger.LogInformation("Claude grant discovery returned {Count} grants using {Tokens} tokens",
                    grants.Count, tokensUsed);

                return new GrantDiscoveryResult
                {
                    Grants = grants,
                    TokensUsed = tokensUsed,
                };
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse Claude response as JSON");
                return new GrantDiscoveryResult
                {
                    Message = "AI returned a response that could not be parsed. Try rephrasing your query.",
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calling Claude API for grant discovery");
                return new GrantDiscoveryResult
                {
                    Message = $"AI discovery failed: {ex.Message}",
                };
            }
            finally
            {
                RateLimiter.Release();
            }
        }

        private static string BuildUserPrompt(GrantDiscoveryRequest request, int maxResults)
        {
            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                // Freeform mode: admin wrote a custom research query
                return $"""
                    {request.Prompt}

                    Return up to {maxResults} results as a JSON array.
                    """;
            }

            // Focus-area mode: build a prompt from selected areas
            var focusAreas = string.IsNullOrWhiteSpace(request.FocusAreas)
                ? "environmental cleanup, conservation, community development"
                : request.FocusAreas;

            return $"""
                Find up to {maxResults} grant opportunities relevant to a 501(c)(3) nonprofit
                focused on community environmental cleanup events.

                Focus areas: {focusAreas}

                Include grants from federal agencies (EPA, USDA, etc.), state environmental agencies,
                private foundations, and corporate giving programs. Prioritize grants that are
                currently accepting applications or have upcoming deadlines.
                """;
        }
    }
}
