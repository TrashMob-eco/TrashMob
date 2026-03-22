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
    public class OutreachContentService(
        IConfiguration configuration,
        ILogger<OutreachContentService> logger) : IOutreachContentService
    {
        private static readonly SemaphoreSlim RateLimiter = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private const string SystemPrompt = """
            You are an email copywriter for TrashMob.eco, a 501(c)(3) nonprofit that helps communities
            organize volunteer litter cleanup events. You write outreach emails to potential community
            partners (cities, HOAs, nonprofits, civic organizations).

            ABOUT TRASHMOB:
            - TrashMob.eco is a registered 501(c)(3) nonprofit, not a commercial vendor.
            - The platform is built by volunteers who care about clean communities.
            - Communities use TrashMob to coordinate volunteers, manage events, track environmental
              impact, handle liability waivers, and engage residents — replacing manual spreadsheets
              and email chains.

            WRITING GUIDELINES:
            - Lead with the prospect's problem or goal, not TrashMob's features.
            - Keep emails SHORT: initial outreach under 150 words, follow-ups under 100 words.
            - Write 3-4 short paragraphs max. No bullet-point feature dumps.
            - Pick 1-2 benefits most relevant to the prospect type, not a laundry list.
            - Use concrete numbers when provided (nearby events, volunteers, etc.).
            - Do NOT claim the platform is free. Communities have partnership plans at various tiers.
            - End with a specific call to action: reply to schedule a 15-minute call.
            - Sign off as "Joe Beernink, Founder, TrashMob.eco" — this is a personal outreach.
            - Tone: genuine, conversational, mission-driven. Write like a neighbor, not a salesperson.
            - No corporate buzzwords. No "synergy," "leverage," "streamline," or "comprehensive solution."

            You must return ONLY a JSON object with these fields:
            - "subject": string (email subject line, max 60 characters, no generic phrases like "Partnership Opportunity")
            - "htmlBody": string (HTML email body, use <p> tags for paragraphs, minimal formatting)

            Return ONLY the JSON object, no markdown fences, no other text.
            """;

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
                    Address it to {contactName}.
                    {(nearbyEventCount > 0 ? $"Mention that {nearbyEventCount} volunteer cleanup events have already happened near {location} through TrashMob." : "")}

                    Open with something specific to {location} or the challenges a {prospectType} faces
                    with litter or volunteer coordination. Then briefly introduce TrashMob as a nonprofit
                    that helps solve that problem. Pick the 1-2 benefits most relevant to a {prospectType}.
                    End by asking if they'd like a 15-minute call to learn more.
                    Under 150 words.
                    """,

                2 => $"""
                    Write a short follow-up email to {prospect.Name} ({prospectType}) in {location}.
                    Address it to {contactName}. We emailed a few days ago.

                    Don't repeat the first email. Pick ONE concrete outcome that would matter to a
                    {prospectType} (e.g., reducing admin time, engaging more residents, tracking impact
                    for grants/reports) and frame it as a question. Under 100 words.
                    """,

                3 => $"""
                    Write a value-add email to {prospect.Name} ({prospectType}) in {location}.
                    Address it to {contactName}. We've reached out twice without a response.
                    {(nearbyEventCount > 0 ? $"{nearbyEventCount} volunteer cleanups have happened near {location} through TrashMob." : "")}

                    Share one specific example of how communities use TrashMob (e.g., a city that
                    replaced spreadsheet sign-ups, or an HOA that tracked volunteer hours for grant
                    reporting). Frame it as "here's what others in your position are doing."
                    Under 100 words.
                    """,

                4 => $"""
                    Write a final follow-up email to {prospect.Name} ({prospectType}) in {location}.
                    Address it to {contactName}. This is our last outreach.

                    Two to three sentences max. Acknowledge they're busy, say we'd love to help when
                    the timing is right, and leave the door open. No guilt, no pressure.
                    Under 60 words.
                    """,

                _ => $"""
                    Write an outreach email to {prospect.Name}, a {prospectType} in {location}.
                    Address it to {contactName}.

                    Briefly introduce TrashMob.eco as a nonprofit and explain how it helps communities
                    like theirs. Under 150 words.
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
