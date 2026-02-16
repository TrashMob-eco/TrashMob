#nullable enable

namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class NominatimService(HttpClient httpClient, ILogger<NominatimService> logger) : INominatimService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<NominatimResult?> SearchWithPolygonAsync(
            string query,
            (double North, double South, double East, double West)? viewBox = null,
            CancellationToken cancellationToken = default)
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=jsonv2&polygon_geojson=1&limit=1";

            if (viewBox.HasValue)
            {
                var vb = viewBox.Value;
                url += $"&viewbox={vb.West.ToString(CultureInfo.InvariantCulture)},{vb.North.ToString(CultureInfo.InvariantCulture)},{vb.East.ToString(CultureInfo.InvariantCulture)},{vb.South.ToString(CultureInfo.InvariantCulture)}&bounded=1";
            }

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrashMob.eco/1.0");

            logger.LogInformation("Nominatim search: {Query}", query);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Nominatim search failed with status {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var results = JsonSerializer.Deserialize<List<NominatimApiResult>>(content, JsonOptions);

            if (results is null || results.Count == 0)
            {
                logger.LogInformation("Nominatim returned no results for: {Query}", query);
                return null;
            }

            var top = results[0];

            if (top.Geojson is null)
            {
                logger.LogInformation("Nominatim result has no GeoJSON geometry for: {Query}", query);
                return null;
            }

            var geoType = top.Geojson.Type;

            // We only support Polygon and LineString in the frontend.
            // Convert MultiPolygon to Polygon by taking the largest ring.
            if (string.Equals(geoType, "MultiPolygon", StringComparison.OrdinalIgnoreCase))
            {
                var converted = ConvertMultiPolygonToPolygon(top.Geojson);
                if (converted is null)
                {
                    return null;
                }

                return new NominatimResult
                {
                    GeoJson = JsonSerializer.Serialize(converted, JsonOptions),
                    DisplayName = top.DisplayName ?? string.Empty,
                    Category = top.Category ?? string.Empty,
                    Type = top.Type ?? string.Empty,
                };
            }

            if (string.Equals(geoType, "Polygon", StringComparison.OrdinalIgnoreCase)
                || string.Equals(geoType, "LineString", StringComparison.OrdinalIgnoreCase))
            {
                return new NominatimResult
                {
                    GeoJson = JsonSerializer.Serialize(top.Geojson, JsonOptions),
                    DisplayName = top.DisplayName ?? string.Empty,
                    Category = top.Category ?? string.Empty,
                    Type = top.Type ?? string.Empty,
                };
            }

            // Point or other geometry types — not useful for area boundaries
            logger.LogInformation("Nominatim result geometry type '{Type}' not supported for area boundaries", geoType);
            return null;
        }

        /// <summary>
        /// Converts a MultiPolygon to a single Polygon by selecting the ring with the most coordinates.
        /// </summary>
        private static NominatimGeoJson? ConvertMultiPolygonToPolygon(NominatimGeoJson multiPolygon)
        {
            if (multiPolygon.Coordinates is null)
            {
                return null;
            }

            // MultiPolygon coordinates: JsonElement representing number[][][][]
            // We need to find the polygon (number[][][]) with the largest outer ring.
            JsonElement? bestPolygon = null;
            var bestCount = 0;

            if (multiPolygon.Coordinates.Value.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var polygon in multiPolygon.Coordinates.Value.EnumerateArray())
            {
                if (polygon.ValueKind != JsonValueKind.Array || polygon.GetArrayLength() == 0)
                {
                    continue;
                }

                var outerRing = polygon[0];
                var count = outerRing.GetArrayLength();
                if (count > bestCount)
                {
                    bestCount = count;
                    bestPolygon = polygon;
                }
            }

            if (bestPolygon is null)
            {
                return null;
            }

            return new NominatimGeoJson
            {
                Type = "Polygon",
                Coordinates = bestPolygon,
            };
        }

        // Internal DTOs for Nominatim API response parsing

        private sealed class NominatimApiResult
        {
            public string? DisplayName { get; set; }
            public string? Category { get; set; }
            public string? Type { get; set; }
            public NominatimGeoJson? Geojson { get; set; }
        }

        private sealed class NominatimGeoJson
        {
            public string Type { get; set; } = string.Empty;
            public JsonElement? Coordinates { get; set; }
        }
    }
}
