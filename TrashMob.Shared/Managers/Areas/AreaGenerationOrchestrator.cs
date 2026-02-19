namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Orchestrates the end-to-end area generation process for a batch:
    /// discover features from OSM → process and deduplicate → stage for review.
    /// </summary>
    public class AreaGenerationOrchestrator(
        IAreaGenerationBatchManager batchManager,
        IKeyedRepository<StagedAdoptableArea> stagedRepo,
        IAdoptableAreaManager adoptableAreaManager,
        INominatimService nominatimService,
        IKeyedRepository<Partner> partnerRepo,
        ILogger<AreaGenerationOrchestrator> logger)
        : IAreaGenerationOrchestrator
    {
        // Centroids within this distance (meters) of an existing area are flagged as potential duplicates
        private const double DuplicateDistanceMeters = 100.0;

        // Length in meters for highway section splitting (~1 mile)
        private const double HighwaySectionLengthMeters = 1609.34;

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        /// <inheritdoc />
        public async Task ExecuteAsync(Guid batchId, CancellationToken cancellationToken = default)
        {
            var batch = await batchManager.GetAsync(batchId, cancellationToken);
            if (batch == null)
            {
                logger.LogError("Area generation batch {BatchId} not found", batchId);
                return;
            }

            try
            {
                // Phase 1: Discover features from OSM
                await UpdateBatchStatusAsync(batch, "Discovering", cancellationToken);

                var bounds = GetBounds(batch);
                if (bounds == null)
                {
                    // Fall back to partner/community bounds
                    var partner = await partnerRepo.GetAsync(batch.PartnerId, cancellationToken);
                    if (partner?.BoundsNorth == null || partner?.BoundsSouth == null
                        || partner?.BoundsEast == null || partner?.BoundsWest == null)
                    {
                        await FailBatchAsync(batch, "Community has no geographic bounds configured.", cancellationToken);
                        return;
                    }

                    bounds = (partner.BoundsNorth.Value, partner.BoundsSouth.Value,
                              partner.BoundsEast.Value, partner.BoundsWest.Value);
                }

                // Discover features: use Overpass for tag-based queries, Nominatim for text search
                var allDiscovered = new List<NominatimResult>();
                var overpassQuery = MapCategoryToOverpassQuery(batch.Category);

                if (!string.IsNullOrEmpty(overpassQuery))
                {
                    var results = await nominatimService.SearchByOverpassAsync(overpassQuery, bounds.Value, cancellationToken);
                    allDiscovered.AddRange(results);
                }
                else
                {
                    var searchQueries = MapCategoryToSearchQueries(batch.Category);
                    foreach (var query in searchQueries)
                    {
                        var results = await nominatimService.SearchByCategoryAsync(query, bounds.Value, cancellationToken);
                        allDiscovered.AddRange(results);
                    }
                }

                // For highway sections, split LineString results into mile-length segments
                List<NominatimResult> discovered;
                if (batch.Category.Equals("HighwaySection", StringComparison.OrdinalIgnoreCase))
                {
                    discovered = SplitHighwaysIntoSections(allDiscovered);
                }
                else
                {
                    discovered = allDiscovered;
                }

                batch.DiscoveredCount = discovered.Count;
                await UpdateBatchStatusAsync(batch, "Processing", cancellationToken);

                if (discovered.Count == 0)
                {
                    batch.CompletedDate = DateTimeOffset.UtcNow;
                    await UpdateBatchStatusAsync(batch, "Complete", cancellationToken);
                    return;
                }

                // Load existing areas for dedup
                var existingAreas = await LoadExistingAreasAsync(batch.PartnerId, cancellationToken);

                // Phase 2: Process each discovered feature
                foreach (var feature in discovered)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    batch.ProcessedCount++;

                    // Skip features with no name
                    var name = !string.IsNullOrWhiteSpace(feature.Name) ? feature.Name.Trim() : null;
                    if (name == null)
                    {
                        batch.SkippedCount++;
                        await batchManager.UpdateAsync(batch, cancellationToken);
                        continue;
                    }

                    // Map category to area type
                    var areaType = MapCategoryToAreaType(batch.Category);

                    // Determine confidence
                    var confidence = DetermineConfidence(feature);

                    // Check for duplicates
                    var (isDuplicate, duplicateOfName) = CheckDuplicate(name, feature.Latitude, feature.Longitude, existingAreas);

                    // Create staged area
                    var staged = new StagedAdoptableArea
                    {
                        Id = Guid.NewGuid(),
                        BatchId = batchId,
                        PartnerId = batch.PartnerId,
                        Name = name,
                        Description = $"{areaType} discovered from OpenStreetMap",
                        AreaType = areaType,
                        GeoJson = feature.GeoJson,
                        CenterLatitude = feature.Latitude,
                        CenterLongitude = feature.Longitude,
                        ReviewStatus = "Pending",
                        Confidence = confidence,
                        IsPotentialDuplicate = isDuplicate,
                        DuplicateOfName = duplicateOfName,
                        OsmId = feature.OsmId,
                        CreatedByUserId = batch.CreatedByUserId,
                        CreatedDate = DateTimeOffset.UtcNow,
                        LastUpdatedByUserId = batch.CreatedByUserId,
                        LastUpdatedDate = DateTimeOffset.UtcNow,
                    };

                    await stagedRepo.AddAsync(staged);
                    batch.StagedCount++;
                    await batchManager.UpdateAsync(batch, cancellationToken);
                }

                // Phase 3: Complete
                batch.CompletedDate = DateTimeOffset.UtcNow;
                await UpdateBatchStatusAsync(batch, "Complete", cancellationToken);

                logger.LogInformation(
                    "Area generation batch {BatchId} complete: {Discovered} discovered, {Staged} staged, {Skipped} skipped",
                    batchId, batch.DiscoveredCount, batch.StagedCount, batch.SkippedCount);
            }
            catch (OperationCanceledException)
            {
                batch.CompletedDate = DateTimeOffset.UtcNow;
                await UpdateBatchStatusAsync(batch, "Cancelled", CancellationToken.None);
                logger.LogInformation("Area generation batch {BatchId} cancelled", batchId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Area generation batch {BatchId} failed", batchId);
                await FailBatchAsync(batch, ex.Message, CancellationToken.None);
            }
        }

        private static (double North, double South, double East, double West)? GetBounds(AreaGenerationBatch batch)
        {
            if (batch.BoundsNorth.HasValue && batch.BoundsSouth.HasValue
                && batch.BoundsEast.HasValue && batch.BoundsWest.HasValue)
            {
                return (batch.BoundsNorth.Value, batch.BoundsSouth.Value,
                        batch.BoundsEast.Value, batch.BoundsWest.Value);
            }

            return null;
        }

        private async Task<List<(string Name, double Lat, double Lon)>> LoadExistingAreasAsync(
            Guid partnerId, CancellationToken cancellationToken)
        {
            // We need name and approximate center for dedup.
            // Areas may not have a center stored directly, but we can use the start coordinates
            // or we'll just do name-based matching.
            var areas = await adoptableAreaManager.GetByCommunityAsync(partnerId, cancellationToken);
            return areas.Select(a => (
                Name: a.Name,
                Lat: a.StartLatitude ?? 0,
                Lon: a.StartLongitude ?? 0
            )).ToList();
        }

        private static (bool IsDuplicate, string DuplicateOfName) CheckDuplicate(
            string name,
            double lat, double lon,
            List<(string Name, double Lat, double Lon)> existingAreas)
        {
            foreach (var existing in existingAreas)
            {
                // Case-insensitive name contains check
                if (existing.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                    || name.Contains(existing.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return (true, existing.Name);
                }

                // Centroid proximity check (~100m)
                if (existing.Lat != 0 && existing.Lon != 0)
                {
                    var distance = HaversineDistance(lat, lon, existing.Lat, existing.Lon);
                    if (distance < DuplicateDistanceMeters)
                    {
                        return (true, existing.Name);
                    }
                }
            }

            return (false, null);
        }

        private static string DetermineConfidence(NominatimResult feature)
        {
            var hasPolygon = !string.IsNullOrEmpty(feature.GeoJson) && feature.GeoJson.Contains("Polygon");
            var hasName = !string.IsNullOrWhiteSpace(feature.Name);

            if (hasName && hasPolygon)
            {
                return "High";
            }

            if (hasName)
            {
                return "Medium";
            }

            return "Low";
        }

        /// <summary>
        /// Maps a user-facing category to an Overpass QL query for tag-based OSM search.
        /// Returns empty string for categories that work better with Nominatim text search.
        /// {{bbox}} is replaced with the actual bounding box at query time.
        /// </summary>
        private static string MapCategoryToOverpassQuery(string category)
        {
            return category.ToLowerInvariant() switch
            {
                "interchange" => "[out:json][timeout:60];(node[\"highway\"=\"motorway_junction\"]({{bbox}}););out body;",
                "cityblock" => "[out:json][timeout:60];(way[\"place\"=\"neighbourhood\"]({{bbox}});relation[\"place\"=\"neighbourhood\"]({{bbox}});way[\"place\"=\"city_block\"]({{bbox}}););out body geom;",
                "highwaysection" => "[out:json][timeout:60];(way[\"highway\"=\"motorway\"]({{bbox}});way[\"highway\"=\"trunk\"]({{bbox}}););out body geom;",
                _ => "", // Use Nominatim text search for school, park, trail, etc.
            };
        }

        /// <summary>
        /// Maps a user-facing category to one or more Nominatim search queries (text-based).
        /// Used as fallback when no Overpass query is defined.
        /// </summary>
        private static List<string> MapCategoryToSearchQueries(string category)
        {
            return [category];
        }

        private static string MapCategoryToAreaType(string category)
        {
            return category.ToLowerInvariant() switch
            {
                "school" => "School",
                "park" => "Park",
                "trail" => "Trail",
                "waterway" => "Waterway",
                "interchange" => "Interchange",
                "cityblock" => "CityBlock",
                "highwaysection" => "HighwaySection",
                _ => "Spot",
            };
        }

        /// <summary>
        /// Splits highway/interstate LineString features into approximately 1-mile segments.
        /// Each segment becomes a separate staged area named with mile markers.
        /// </summary>
        private List<NominatimResult> SplitHighwaysIntoSections(List<NominatimResult> highways)
        {
            var sections = new List<NominatimResult>();

            foreach (var highway in highways)
            {
                if (string.IsNullOrEmpty(highway.GeoJson))
                {
                    continue;
                }

                var coords = ParseLineStringCoordinates(highway.GeoJson);
                if (coords.Count < 2)
                {
                    // Not a LineString or too short — keep as-is
                    sections.Add(highway);
                    continue;
                }

                var totalDistance = CalculatePathDistance(coords);
                if (totalDistance < HighwaySectionLengthMeters * 0.5)
                {
                    // Highway segment is less than half a mile — keep as single area
                    sections.Add(highway);
                    continue;
                }

                var sectionCount = (int)Math.Ceiling(totalDistance / HighwaySectionLengthMeters);
                var currentDistance = 0.0;
                var sectionStart = 0;
                var sectionNumber = 1;

                for (var i = 1; i < coords.Count; i++)
                {
                    currentDistance += HaversineDistance(coords[i - 1].Lat, coords[i - 1].Lon, coords[i].Lat, coords[i].Lon);

                    if (currentDistance >= HighwaySectionLengthMeters || i == coords.Count - 1)
                    {
                        var sectionCoords = coords.GetRange(sectionStart, i - sectionStart + 1);
                        if (sectionCoords.Count >= 2)
                        {
                            var midpoint = sectionCoords[sectionCoords.Count / 2];
                            var sectionGeoJson = BuildLineStringGeoJson(sectionCoords);
                            var baseName = !string.IsNullOrWhiteSpace(highway.Name) ? highway.Name : "Highway";
                            var mileStart = sectionNumber - 1;

                            sections.Add(new NominatimResult
                            {
                                GeoJson = sectionGeoJson,
                                DisplayName = $"{baseName} - Mile {mileStart} to {sectionNumber}",
                                Category = highway.Category,
                                Type = highway.Type,
                                OsmId = $"{highway.OsmId}:section{sectionNumber}",
                                Name = $"{baseName} - Mile {mileStart}-{sectionNumber}",
                                Latitude = midpoint.Lat,
                                Longitude = midpoint.Lon,
                                BoundingBox = null,
                            });
                        }

                        sectionStart = i;
                        currentDistance = 0;
                        sectionNumber++;
                    }
                }

                logger.LogInformation(
                    "Split highway \"{HighwayName}\" ({Distance:F0}m) into {Sections} sections",
                    highway.Name, totalDistance, sectionNumber - 1);
            }

            return sections;
        }

        private static List<(double Lat, double Lon)> ParseLineStringCoordinates(string geoJson)
        {
            try
            {
                using var doc = JsonDocument.Parse(geoJson);
                var root = doc.RootElement;

                if (!root.TryGetProperty("type", out var typeProp))
                {
                    return [];
                }

                var geoType = typeProp.GetString();
                if (!string.Equals(geoType, "LineString", StringComparison.OrdinalIgnoreCase))
                {
                    return [];
                }

                if (!root.TryGetProperty("coordinates", out var coordsProp))
                {
                    return [];
                }

                var result = new List<(double Lat, double Lon)>();
                foreach (var coord in coordsProp.EnumerateArray())
                {
                    var lon = coord[0].GetDouble();
                    var lat = coord[1].GetDouble();
                    result.Add((lat, lon));
                }

                return result;
            }
            catch
            {
                return [];
            }
        }

        private static string BuildLineStringGeoJson(List<(double Lat, double Lon)> coords)
        {
            var coordsArray = coords.Select(c => new[] { c.Lon, c.Lat }).ToArray();
            var geoJsonObj = new { type = "LineString", coordinates = coordsArray };
            return JsonSerializer.Serialize(geoJsonObj, JsonOptions);
        }

        private static double CalculatePathDistance(List<(double Lat, double Lon)> coords)
        {
            var total = 0.0;
            for (var i = 1; i < coords.Count; i++)
            {
                total += HaversineDistance(coords[i - 1].Lat, coords[i - 1].Lon, coords[i].Lat, coords[i].Lon);
            }
            return total;
        }

        private async Task UpdateBatchStatusAsync(AreaGenerationBatch batch, string status, CancellationToken cancellationToken)
        {
            batch.Status = status;
            batch.LastUpdatedDate = DateTimeOffset.UtcNow;
            await batchManager.UpdateAsync(batch, cancellationToken);
        }

        private async Task FailBatchAsync(AreaGenerationBatch batch, string errorMessage, CancellationToken cancellationToken)
        {
            batch.Status = "Failed";
            batch.ErrorMessage = errorMessage.Length > 4000 ? errorMessage[..4000] : errorMessage;
            batch.CompletedDate = DateTimeOffset.UtcNow;
            batch.LastUpdatedDate = DateTimeOffset.UtcNow;
            await batchManager.UpdateAsync(batch, cancellationToken);
        }

        /// <summary>
        /// Calculates the Haversine distance between two points in meters.
        /// </summary>
        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth's radius in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}
