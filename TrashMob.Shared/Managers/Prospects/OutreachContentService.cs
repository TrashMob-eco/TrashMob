namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Anthropic;
    using Anthropic.Models.Messages;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Generates AI-personalized outreach email content using the Anthropic Claude API.
    /// </summary>
    public class OutreachContentService : IOutreachContentService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<OutreachContentService> logger;
        private static readonly SemaphoreSlim RateLimiter = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private const string SystemPrompt = """
            You are an email copywriter for TrashMob.eco, a nonprofit that helps communities organize
            volunteer litter cleanup events. You write professional, warm outreach emails to potential
            community partners (cities, HOAs, nonprofits, civic organizations).

            IMPORTANT GUIDELINES:
            - Do NOT claim TrashMob is free or zero-cost. Communities have subscription plans.
            - Emphasize the VALUE TrashMob provides: volunteer coordination, event management,
              impact tracking, waiver handling, partner logistics, and community engagement.
            - Invite prospects to learn more, not to sign up for a free service.
            - Keep emails concise and professional. No hard-sell tactics.
            - Reference the prospect's city/region and type when possible.
            - Use a friendly but professional tone.

            You must return ONLY a JSON object with these fields:
            - "subject": string (email subject line, max 80 characters)
            - "htmlBody": string (HTML email body content, use <p>, <ul>, <li>, <strong> tags)

            Return ONLY the JSON object, no markdown fences, no other text.
            """;

        public OutreachContentService(IConfiguration configuration,
            ILogger<OutreachContentService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<OutreachPreview> GenerateOutreachContentAsync(CommunityProspect prospect,
            int cadenceStep, int nearbyEventCount, CancellationToken cancellationToken = default)
        {
            var apiKey = configuration["AnthropicApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "x")
            {
                logger.LogWarning("Anthropic API key not configured. Returning fallback outreach content.");
                return BuildFallbackContent(prospect, cadenceStep);
            }

            var userPrompt = BuildPromptForStep(prospect, cadenceStep, nearbyEventCount);

            await RateLimiter.WaitAsync(cancellationToken);
            try
            {
                var client = new AnthropicClient { ApiKey = apiKey };
                var maxTokens = int.TryParse(configuration["AnthropicMaxTokens"], out var mt) ? mt : 2048;

                var response = await client.Messages.Create(new MessageCreateParams
                {
                    Model = Model.ClaudeSonnet4_5_20250929,
                    MaxTokens = maxTokens,
                    System = SystemPrompt,
                    Messages = [new MessageParam { Role = "user", Content = userPrompt }],
                }, cancellationToken: cancellationToken);

                string responseText = "{}";
                if (response.Content?.Count > 0 && response.Content[0].TryPickText(out var textBlock))
                {
                    responseText = textBlock.Text;
                }

                var tokensUsed = (int)((response.Usage?.InputTokens ?? 0) + (response.Usage?.OutputTokens ?? 0));

                // Strip markdown fences if present
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

                var parsed = JsonSerializer.Deserialize<OutreachContentResponse>(responseText, JsonOptions);

                logger.LogInformation(
                    "Generated outreach content for prospect {ProspectId} step {Step} using {Tokens} tokens",
                    prospect.Id, cadenceStep, tokensUsed);

                return new OutreachPreview
                {
                    ProspectId = prospect.Id,
                    ProspectName = prospect.Name,
                    CadenceStep = cadenceStep,
                    Subject = parsed?.Subject ?? $"TrashMob.eco — Community Partnership Opportunity",
                    HtmlBody = parsed?.HtmlBody ?? "<p>We'd love to partner with your community.</p>",
                    TokensUsed = tokensUsed,
                };
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse AI outreach content response");
                return BuildFallbackContent(prospect, cadenceStep);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calling Claude API for outreach content generation");
                return BuildFallbackContent(prospect, cadenceStep);
            }
            finally
            {
                RateLimiter.Release();
            }
        }

        private static string BuildPromptForStep(CommunityProspect prospect, int cadenceStep, int nearbyEventCount)
        {
            var location = !string.IsNullOrWhiteSpace(prospect.City)
                ? $"{prospect.City}, {prospect.Region}"
                : prospect.Region ?? "their area";

            var prospectType = prospect.Type ?? "community organization";
            var contactName = prospect.ContactName ?? "the appropriate contact";

            return cadenceStep switch
            {
                1 => $"""
                    Write an initial outreach email to {prospect.Name}, a {prospectType} in {location}.
                    Contact person: {contactName}.
                    {(nearbyEventCount > 0 ? $"There are {nearbyEventCount} TrashMob cleanup events happening near {location}." : "")}

                    Introduce TrashMob.eco and explain how partnering can help their community with
                    volunteer-organized litter cleanups. Emphasize the value of volunteer coordination,
                    impact tracking, liability waiver management, and logistics support.
                    Invite them to learn more about community partnership plans.
                    """,

                2 => $"""
                    Write a brief follow-up email to {prospect.Name}, a {prospectType} in {location}.
                    We sent an initial outreach email a few days ago.
                    Contact person: {contactName}.

                    Keep it short — just checking in and highlighting one key benefit relevant to a
                    {prospectType}: volunteer coordination, event management, or community engagement.
                    """,

                3 => $"""
                    Write a value-add email to {prospect.Name}, a {prospectType} in {location}.
                    We've reached out before but haven't heard back.
                    Contact person: {contactName}.
                    {(nearbyEventCount > 0 ? $"There are {nearbyEventCount} TrashMob events in their area." : "")}

                    Share how TrashMob.eco complements their existing community programs. Mention
                    impact tracking, volunteer management, event coordination, and how other communities
                    have benefited. Position TrashMob as a partner that adds value to what they already do.
                    """,

                4 => $"""
                    Write a final gentle follow-up email to {prospect.Name}, a {prospectType} in {location}.
                    We've sent several emails without a response.
                    Contact person: {contactName}.

                    Keep it very brief and respectful. Offer to connect directly if they're interested,
                    and let them know we're here whenever the timing is right. No pressure.
                    """,

                _ => $"""
                    Write an outreach email to {prospect.Name}, a {prospectType} in {location}.
                    Contact person: {contactName}.

                    Introduce TrashMob.eco and explain the value of community partnership.
                    """
            };
        }

        private static OutreachPreview BuildFallbackContent(CommunityProspect prospect, int cadenceStep)
        {
            var subject = cadenceStep switch
            {
                1 => $"TrashMob.eco — Partnering with {prospect.Name} for Cleaner Communities",
                2 => $"Following up — TrashMob.eco Partnership with {prospect.Name}",
                3 => $"How TrashMob.eco supports communities like {prospect.Name}",
                4 => $"Still interested? TrashMob.eco + {prospect.Name}",
                _ => $"TrashMob.eco — Community Partnership Opportunity",
            };

            var htmlBody = $"<p>Hello,</p><p>We'd love to explore how TrashMob.eco can support " +
                $"{prospect.Name} with volunteer-organized litter cleanup events in your community.</p>" +
                $"<p>Visit <a href=\"https://www.trashmob.eco/partnerships\">our partnerships page</a> to learn more.</p>";

            return new OutreachPreview
            {
                ProspectId = prospect.Id,
                ProspectName = prospect.Name,
                CadenceStep = cadenceStep,
                Subject = subject,
                HtmlBody = htmlBody,
                TokensUsed = 0,
            };
        }

        /// <summary>
        /// Internal class for deserializing the AI response.
        /// </summary>
#nullable enable
        private sealed class OutreachContentResponse
        {
            public string? Subject { get; set; }
            public string? HtmlBody { get; set; }
        }
#nullable restore
    }
}
