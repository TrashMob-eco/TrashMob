#nullable enable

namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Anthropic;
    using Anthropic.Models.Messages;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class AreaSuggestionService(
        IConfiguration configuration,
        ILogger<AreaSuggestionService> logger,
        IMapManager mapManager,
        INominatimService nominatimService) : IAreaSuggestionService
    {
        private static readonly SemaphoreSlim RateLimiter = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private const string SystemPrompt = """
            You are a geographic area interpreter for TrashMob.eco, a community cleanup platform.
            Users describe areas they want to adopt for cleanup. Your job is to convert their
            natural language description into structured geocoding queries.

            You must return ONLY a JSON object with these fields:
            - "queries": array of objects, each with:
              - "address": string — a specific address or intersection to geocode
              - "type": string — either "point" (single location) or "street_segment" (a stretch of road)
            - "suggestedName": string — a concise, descriptive name for this area (e.g. "200 Block of Main St")
            - "suggestedAreaType": string — one of: "Highway", "Park", "School", "Trail", "Waterway", "Street", "Spot"
            - "geometryType": string — either "polygon" (for parks, lots, blocks) or "linestring" (for streets, trails, waterways)
            - "confidence": number — your confidence in the interpretation, from 0.0 to 1.0
            - "isNamedFeature": boolean — true if the description refers to a specific named place
              (park, school, building, lake, etc.) that likely has a well-defined boundary in OpenStreetMap

            Guidelines:
            - For street segments: provide start and end intersection addresses as two queries with type "street_segment"
            - For parks or landmarks: provide the park/landmark name as a single query with type "point"
            - For blocks: provide the two nearest cross-street intersections as queries
            - Include the city/community name in each address for accurate geocoding
            - If the description is ambiguous, use your best interpretation and lower the confidence

            Return ONLY the JSON object, no markdown fences, no other text.
            """;

        public async Task<AreaSuggestionResult> SuggestAreaAsync(
            AreaSuggestionRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return new AreaSuggestionResult { Message = "Please provide a description of the area." };
            }

            var apiKey = configuration["AnthropicApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "x")
            {
                logger.LogWarning("Anthropic API key not configured. AI area suggestion unavailable.");
                return new AreaSuggestionResult { Message = "AI area suggestion is not configured." };
            }

            await RateLimiter.WaitAsync(cancellationToken);
            try
            {
                // Step 1: Ask Claude to interpret the description
                var claudeResponse = await GetClaudeInterpretation(apiKey, request, cancellationToken);
                if (claudeResponse is null)
                {
                    return new AreaSuggestionResult { Message = "Could not interpret the area description. Try being more specific." };
                }

                // Build viewport bounds if provided
                (double North, double South, double East, double West)? geoBounds = null;
                if (request.BoundsNorth.HasValue && request.BoundsSouth.HasValue
                    && request.BoundsEast.HasValue && request.BoundsWest.HasValue)
                {
                    geoBounds = (request.BoundsNorth.Value, request.BoundsSouth.Value,
                        request.BoundsEast.Value, request.BoundsWest.Value);
                }

                // Step 2: For named features, try OSM Nominatim for real boundary polygons
                if (claudeResponse.IsNamedFeature && claudeResponse.Queries?.Count > 0)
                {
                    var nominatimGeoJson = await TryNominatimLookup(
                        claudeResponse.Queries[0].Address, geoBounds, cancellationToken);

                    if (nominatimGeoJson is not null)
                    {
                        return new AreaSuggestionResult
                        {
                            GeoJson = nominatimGeoJson,
                            SuggestedName = claudeResponse.SuggestedName,
                            SuggestedAreaType = claudeResponse.SuggestedAreaType,
                            Confidence = claudeResponse.Confidence,
                        };
                    }
                }

                // Step 3: Fall back to Azure Maps geocoding
                var coordinates = await GeocodeQueries(claudeResponse.Queries, geoBounds, cancellationToken);
                if (coordinates.Count == 0)
                {
                    return new AreaSuggestionResult
                    {
                        Message = "Could not find coordinates for the described area. Try including a city name or more specific landmarks.",
                        SuggestedName = claudeResponse.SuggestedName,
                        SuggestedAreaType = claudeResponse.SuggestedAreaType,
                        Confidence = claudeResponse.Confidence,
                    };
                }

                // Step 4: Build GeoJSON from coordinates
                var geoJson = BuildGeoJson(coordinates, claudeResponse.GeometryType);

                return new AreaSuggestionResult
                {
                    GeoJson = geoJson,
                    SuggestedName = claudeResponse.SuggestedName,
                    SuggestedAreaType = claudeResponse.SuggestedAreaType,
                    Confidence = claudeResponse.Confidence,
                };
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse Claude response for area suggestion");
                return new AreaSuggestionResult
                {
                    Message = "AI returned a response that could not be parsed. Try rephrasing your description.",
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during AI area suggestion");
                return new AreaSuggestionResult
                {
                    Message = $"Area suggestion failed: {ex.Message}",
                };
            }
            finally
            {
                RateLimiter.Release();
            }
        }

        private async Task<ClaudeAreaResponse?> GetClaudeInterpretation(
            string apiKey,
            AreaSuggestionRequest request,
            CancellationToken cancellationToken)
        {
            var userPrompt = BuildUserPrompt(request);

            var client = new AnthropicClient { ApiKey = apiKey };
            var maxTokens = int.TryParse(configuration["AnthropicMaxTokens"], out var mt) ? mt : 4096;

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
            logger.LogInformation("Claude area suggestion used {Tokens} tokens", tokensUsed);

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

            return JsonSerializer.Deserialize<ClaudeAreaResponse>(responseText, JsonOptions);
        }

        private static string BuildUserPrompt(AreaSuggestionRequest request)
        {
            List<string> parts =
            [
                $"Area description: \"{request.Description}\"",
            ];

            if (!string.IsNullOrWhiteSpace(request.CommunityName))
            {
                parts.Add($"Community: {request.CommunityName}");
            }

            if (request.CenterLatitude.HasValue && request.CenterLongitude.HasValue)
            {
                parts.Add($"Community center coordinates: {request.CenterLatitude.Value:F6}, {request.CenterLongitude.Value:F6}");
            }

            if (request.BoundsNorth.HasValue && request.BoundsSouth.HasValue
                && request.BoundsEast.HasValue && request.BoundsWest.HasValue)
            {
                parts.Add($"Viewport bounds: North={request.BoundsNorth.Value:F6}, South={request.BoundsSouth.Value:F6}, East={request.BoundsEast.Value:F6}, West={request.BoundsWest.Value:F6}. The user is looking at this area on the map, so results should be within or near these bounds.");
            }

            return string.Join("\n", parts);
        }

        private async Task<string?> TryNominatimLookup(
            string query,
            (double North, double South, double East, double West)? viewBox,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await nominatimService.SearchWithPolygonAsync(query, viewBox, cancellationToken);
                if (result is not null)
                {
                    logger.LogInformation("Nominatim returned polygon for '{Query}' (category: {Category}, type: {Type})",
                        query, result.Category, result.Type);
                    return result.GeoJson;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Nominatim lookup failed for '{Query}', falling back to Azure Maps", query);
            }

            return null;
        }

        private async Task<List<(double Lat, double Lon)>> GeocodeQueries(
            List<GeocodingQuery>? queries,
            (double North, double South, double East, double West)? boundingBox,
            CancellationToken cancellationToken)
        {
            List<(double Lat, double Lon)> coordinates = [];

            if (queries is null || queries.Count == 0)
            {
                return coordinates;
            }

            foreach (var query in queries)
            {
                try
                {
                    var rawJson = await mapManager.SearchAddressAsync(query.Address, boundingBox: boundingBox);
                    var searchResult = JsonSerializer.Deserialize<AzureMapsSearchResponse>(rawJson, JsonOptions);

                    if (searchResult?.Results is not null && searchResult.Results.Count > 0)
                    {
                        var topResult = searchResult.Results[0];
                        coordinates.Add((topResult.Position.Lat, topResult.Position.Lon));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to geocode query: {Address}", query.Address);
                }
            }

            return coordinates;
        }

        private static string BuildGeoJson(List<(double Lat, double Lon)> coordinates, string? geometryType)
        {
            if (geometryType?.Equals("linestring", StringComparison.OrdinalIgnoreCase) == true
                && coordinates.Count >= 2)
            {
                // Build a LineString
                var coords = coordinates
                    .Select(c => $"[{c.Lon.ToString(CultureInfo.InvariantCulture)},{c.Lat.ToString(CultureInfo.InvariantCulture)}]");
                return $"{{\"type\":\"LineString\",\"coordinates\":[{string.Join(",", coords)}]}}";
            }

            // Build a Polygon
            if (coordinates.Count == 1)
            {
                // Single point: create a small rectangular polygon around it (~100m)
                var (lat, lon) = coordinates[0];
                const double offset = 0.0005; // ~50m
                return BuildRectangleGeoJson(lat - offset, lon - offset, lat + offset, lon + offset);
            }

            if (coordinates.Count == 2)
            {
                // Two points: create a rectangle from their bounding box with padding
                var minLat = Math.Min(coordinates[0].Lat, coordinates[1].Lat);
                var maxLat = Math.Max(coordinates[0].Lat, coordinates[1].Lat);
                var minLon = Math.Min(coordinates[0].Lon, coordinates[1].Lon);
                var maxLon = Math.Max(coordinates[0].Lon, coordinates[1].Lon);

                // Add small padding
                const double padding = 0.0002; // ~20m
                return BuildRectangleGeoJson(minLat - padding, minLon - padding, maxLat + padding, maxLon + padding);
            }

            // 3+ points: close the polygon ring
            var polyCoords = coordinates
                .Select(c => $"[{c.Lon.ToString(CultureInfo.InvariantCulture)},{c.Lat.ToString(CultureInfo.InvariantCulture)}]")
                .ToList();
            // Close the ring by repeating the first point
            polyCoords.Add(polyCoords[0]);
            return $"{{\"type\":\"Polygon\",\"coordinates\":[[{string.Join(",", polyCoords)}]]}}";
        }

        private static string BuildRectangleGeoJson(double south, double west, double north, double east)
        {
            var sw = $"[{west.ToString(CultureInfo.InvariantCulture)},{south.ToString(CultureInfo.InvariantCulture)}]";
            var nw = $"[{west.ToString(CultureInfo.InvariantCulture)},{north.ToString(CultureInfo.InvariantCulture)}]";
            var ne = $"[{east.ToString(CultureInfo.InvariantCulture)},{north.ToString(CultureInfo.InvariantCulture)}]";
            var se = $"[{east.ToString(CultureInfo.InvariantCulture)},{south.ToString(CultureInfo.InvariantCulture)}]";
            return $"{{\"type\":\"Polygon\",\"coordinates\":[[{sw},{nw},{ne},{se},{sw}]]}}";
        }

        // Internal DTOs for Claude response parsing

        private sealed class ClaudeAreaResponse
        {
            public List<GeocodingQuery>? Queries { get; set; }
            public string? SuggestedName { get; set; }
            public string? SuggestedAreaType { get; set; }
            public string? GeometryType { get; set; }
            public double Confidence { get; set; }
            public bool IsNamedFeature { get; set; }
        }

        private sealed class GeocodingQuery
        {
            public string Address { get; set; } = string.Empty;
            public string Type { get; set; } = "point";
        }

        // Internal DTOs for Azure Maps response parsing

        private sealed class AzureMapsSearchResponse
        {
            public List<AzureMapsSearchResult>? Results { get; set; }
        }

        private sealed class AzureMapsSearchResult
        {
            public AzureMapsPosition Position { get; set; } = new();
        }

        private sealed class AzureMapsPosition
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}
