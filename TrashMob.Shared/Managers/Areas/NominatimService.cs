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

        /// <inheritdoc />
        public async Task<IEnumerable<NominatimResult>> SearchByCategoryAsync(
            string category,
            (double North, double South, double East, double West) bounds,
            CancellationToken cancellationToken = default)
        {
            var allResults = new List<NominatimResult>();
            var excludePlaceIds = new List<string>();
            const int maxPages = 10; // Safety limit: 10 pages × 50 = 500 max features

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrashMob.eco/1.0");

            for (var page = 0; page < maxPages; page++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var url = $"https://nominatim.openstreetmap.org/search" +
                    $"?q={Uri.EscapeDataString(category)}" +
                    $"&format=jsonv2" +
                    $"&polygon_geojson=1" +
                    $"&addressdetails=1" +
                    $"&limit=50" +
                    $"&viewbox={bounds.West.ToString(CultureInfo.InvariantCulture)},{bounds.North.ToString(CultureInfo.InvariantCulture)},{bounds.East.ToString(CultureInfo.InvariantCulture)},{bounds.South.ToString(CultureInfo.InvariantCulture)}" +
                    $"&bounded=1";

                if (excludePlaceIds.Count > 0)
                {
                    url += $"&exclude_place_ids={string.Join(",", excludePlaceIds)}";
                }

                logger.LogInformation("Nominatim category search page {Page}: {Category} ({Count} excluded)", page, category, excludePlaceIds.Count);

                // Rate limit: 1 request per second
                if (page > 0)
                {
                    await Task.Delay(1000, cancellationToken);
                }

                var response = await httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Nominatim category search failed with status {StatusCode}", response.StatusCode);
                    break;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var results = JsonSerializer.Deserialize<List<NominatimCategoryApiResult>>(content, JsonOptions);

                if (results is null || results.Count == 0)
                {
                    logger.LogInformation("Nominatim returned no more results for category: {Category}", category);
                    break;
                }

                foreach (var item in results)
                {
                    if (item.PlaceId is not null)
                    {
                        excludePlaceIds.Add(item.PlaceId);
                    }

                    string? geoJson = null;

                    if (item.Geojson is not null)
                    {
                        var geoType = item.Geojson.Type;

                        if (string.Equals(geoType, "MultiPolygon", StringComparison.OrdinalIgnoreCase))
                        {
                            var converted = ConvertMultiPolygonToPolygon(item.Geojson);
                            if (converted is not null)
                            {
                                geoJson = JsonSerializer.Serialize(converted, JsonOptions);
                            }
                        }
                        else if (string.Equals(geoType, "Polygon", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(geoType, "LineString", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(geoType, "Point", StringComparison.OrdinalIgnoreCase))
                        {
                            geoJson = JsonSerializer.Serialize(item.Geojson, JsonOptions);
                        }
                    }

                    _ = double.TryParse(item.Lat, CultureInfo.InvariantCulture, out var lat);
                    _ = double.TryParse(item.Lon, CultureInfo.InvariantCulture, out var lon);

                    allResults.Add(new NominatimResult
                    {
                        GeoJson = geoJson ?? string.Empty,
                        DisplayName = item.DisplayName ?? string.Empty,
                        Category = item.Category ?? string.Empty,
                        Type = item.Type ?? string.Empty,
                        OsmId = $"{item.OsmType ?? ""}:{item.OsmId ?? ""}",
                        Name = item.Name ?? item.DisplayName ?? string.Empty,
                        Latitude = lat,
                        Longitude = lon,
                        BoundingBox = item.Boundingbox is not null ? JsonSerializer.Serialize(item.Boundingbox, JsonOptions) : null,
                    });
                }

                // If fewer than 50 results, we've exhausted the search
                if (results.Count < 50)
                {
                    break;
                }
            }

            logger.LogInformation("Nominatim category search complete: {Category}, {Count} results", category, allResults.Count);
            return allResults;
        }

        /// <inheritdoc />
        public async Task<(double North, double South, double East, double West)?> LookupBoundsAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=jsonv2&limit=1";

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrashMob.eco/1.0");

            logger.LogInformation("Nominatim bounds lookup: {Query}", query);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Nominatim bounds lookup failed with status {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var results = JsonSerializer.Deserialize<List<NominatimApiResult>>(content, JsonOptions);

            var bb = results is not null && results.Count > 0 ? results[0].Boundingbox : null;

            if (bb is null || bb.Length < 4)
            {
                logger.LogInformation("Nominatim returned no bounding box for: {Query}", query);
                return null;
            }

            // Nominatim boundingbox format: [south_lat, north_lat, west_lon, east_lon]
            if (double.TryParse(bb[0], CultureInfo.InvariantCulture, out var south)
                && double.TryParse(bb[1], CultureInfo.InvariantCulture, out var north)
                && double.TryParse(bb[2], CultureInfo.InvariantCulture, out var west)
                && double.TryParse(bb[3], CultureInfo.InvariantCulture, out var east))
            {
                return (north, south, east, west);
            }

            logger.LogWarning("Failed to parse Nominatim bounding box values for: {Query}", query);
            return null;
        }

        /// <inheritdoc />
        public async Task<BoundsWithGeometry?> LookupBoundsWithGeometryAsync(
            string query,
            CancellationToken cancellationToken = default)
        {
            // Single call with polygon_geojson=1 to get both bounds and geometry
            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=jsonv2&polygon_geojson=1&limit=1";

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrashMob.eco/1.0");

            logger.LogInformation("Nominatim bounds+geometry lookup: {Query}", query);

            var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Nominatim bounds+geometry lookup failed with status {StatusCode}", response.StatusCode);
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

            // Parse bounding box
            var bb = top.Boundingbox;
            if (bb is null || bb.Length < 4)
            {
                logger.LogInformation("Nominatim returned no bounding box for: {Query}", query);
                return null;
            }

            if (!double.TryParse(bb[0], CultureInfo.InvariantCulture, out var south)
                || !double.TryParse(bb[1], CultureInfo.InvariantCulture, out var north)
                || !double.TryParse(bb[2], CultureInfo.InvariantCulture, out var west)
                || !double.TryParse(bb[3], CultureInfo.InvariantCulture, out var east))
            {
                logger.LogWarning("Failed to parse Nominatim bounding box values for: {Query}", query);
                return null;
            }

            // Extract GeoJSON polygon if available
            string? geoJson = null;
            if (top.Geojson is not null)
            {
                var geoType = top.Geojson.Type;

                if (string.Equals(geoType, "MultiPolygon", StringComparison.OrdinalIgnoreCase))
                {
                    var converted = ConvertMultiPolygonToPolygon(top.Geojson);
                    if (converted is not null)
                    {
                        geoJson = JsonSerializer.Serialize(converted, JsonOptions);
                    }
                }
                else if (string.Equals(geoType, "Polygon", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(geoType, "LineString", StringComparison.OrdinalIgnoreCase))
                {
                    geoJson = JsonSerializer.Serialize(top.Geojson, JsonOptions);
                }
            }

            return new BoundsWithGeometry
            {
                North = north,
                South = south,
                East = east,
                West = west,
                GeoJson = geoJson,
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<NominatimResult>> SearchByOverpassAsync(
            string overpassQuery,
            (double North, double South, double East, double West) bounds,
            CancellationToken cancellationToken = default)
        {
            var allResults = new List<NominatimResult>();

            var south = bounds.South.ToString(CultureInfo.InvariantCulture);
            var west = bounds.West.ToString(CultureInfo.InvariantCulture);
            var north = bounds.North.ToString(CultureInfo.InvariantCulture);
            var east = bounds.East.ToString(CultureInfo.InvariantCulture);
            var bbox = $"{south},{west},{north},{east}";

            // Build the full Overpass QL query — caller provides the filter body with {{bbox}} placeholders
            var fullQuery = overpassQuery.Replace("{{bbox}}", bbox, StringComparison.OrdinalIgnoreCase);

            logger.LogInformation("Overpass query: {Query}", fullQuery);

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrashMob.eco/1.0");

            var requestContent = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("data", fullQuery),
            ]);

            var response = await httpClient.PostAsync("https://overpass-api.de/api/interpreter", requestContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Overpass query failed with status {StatusCode}", response.StatusCode);
                return allResults;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("elements", out var elements))
            {
                return allResults;
            }

            foreach (var element in elements.EnumerateArray())
            {
                var type = element.GetProperty("type").GetString();
                var id = element.TryGetProperty("id", out var idProp) ? idProp.GetInt64().ToString() : "";
                var osmId = $"{type}:{id}";

                // Get name from tags — try name, then build from ref/description
                string name = "";
                string refTag = "";
                if (element.TryGetProperty("tags", out var tags))
                {
                    if (tags.TryGetProperty("name", out var nameProp))
                    {
                        name = nameProp.GetString() ?? "";
                    }

                    if (tags.TryGetProperty("ref", out var refProp))
                    {
                        refTag = refProp.GetString() ?? "";
                    }

                    // For motorway junctions, build name from ref (e.g., "Exit 17")
                    if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(refTag))
                    {
                        var highway = tags.TryGetProperty("highway", out var hwProp) ? hwProp.GetString() : null;
                        if (string.Equals(highway, "motorway_junction", StringComparison.OrdinalIgnoreCase))
                        {
                            name = $"Exit {refTag}";
                        }
                        else
                        {
                            name = refTag;
                        }
                    }

                    // Last resort: try description
                    if (string.IsNullOrWhiteSpace(name) && tags.TryGetProperty("description", out var descProp))
                    {
                        name = descProp.GetString() ?? "";
                    }
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue; // Skip truly unnamed features
                }

                double lat = 0, lon = 0;
                string geoJson = "";

                if (string.Equals(type, "node", StringComparison.OrdinalIgnoreCase))
                {
                    lat = element.TryGetProperty("lat", out var latProp) ? latProp.GetDouble() : 0;
                    lon = element.TryGetProperty("lon", out var lonProp) ? lonProp.GetDouble() : 0;
                    geoJson = JsonSerializer.Serialize(new { type = "Point", coordinates = new[] { lon, lat } }, JsonOptions);
                }
                else if (element.TryGetProperty("geometry", out var geometry))
                {
                    // Ways/relations with out geom; have a geometry array
                    var coords = new List<double[]>();
                    double sumLat = 0, sumLon = 0;
                    var count = 0;

                    foreach (var point in geometry.EnumerateArray())
                    {
                        var pLat = point.TryGetProperty("lat", out var pLatProp) ? pLatProp.GetDouble() : 0;
                        var pLon = point.TryGetProperty("lon", out var pLonProp) ? pLonProp.GetDouble() : 0;
                        coords.Add([pLon, pLat]);
                        sumLat += pLat;
                        sumLon += pLon;
                        count++;
                    }

                    if (count > 0)
                    {
                        lat = sumLat / count;
                        lon = sumLon / count;
                    }

                    if (coords.Count >= 2)
                    {
                        geoJson = JsonSerializer.Serialize(new { type = "LineString", coordinates = coords }, JsonOptions);
                    }
                }

                allResults.Add(new NominatimResult
                {
                    GeoJson = geoJson,
                    DisplayName = name,
                    Category = "",
                    Type = type ?? "",
                    OsmId = osmId,
                    Name = name,
                    Latitude = lat,
                    Longitude = lon,
                });
            }

            logger.LogInformation("Overpass search complete: {Count} results", allResults.Count);
            return allResults;
        }

        // Internal DTOs for Nominatim API response parsing

        private sealed class NominatimApiResult
        {
            public string? DisplayName { get; set; }
            public string? Category { get; set; }
            public string? Type { get; set; }
            public NominatimGeoJson? Geojson { get; set; }
            public string[]? Boundingbox { get; set; }
        }

        private sealed class NominatimCategoryApiResult
        {
            public string? PlaceId { get; set; }
            public string? OsmType { get; set; }
            public string? OsmId { get; set; }
            public string? DisplayName { get; set; }
            public string? Name { get; set; }
            public string? Category { get; set; }
            public string? Type { get; set; }
            public string? Lat { get; set; }
            public string? Lon { get; set; }
            public string[]? Boundingbox { get; set; }
            public NominatimGeoJson? Geojson { get; set; }
        }

        private sealed class NominatimGeoJson
        {
            public string Type { get; set; } = string.Empty;
            public JsonElement? Coordinates { get; set; }
        }
    }
}
