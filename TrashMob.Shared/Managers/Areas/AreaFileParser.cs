#nullable enable

namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.Extensions.Logging;
    using NetTopologySuite.Features;
    using NetTopologySuite.IO.Esri;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Parses uploaded area files (GeoJSON, KML, KMZ, Shapefile) into normalized features.
    /// </summary>
    public class AreaFileParser(ILogger<AreaFileParser> logger) : IAreaFileParser
    {

        private const int MaxFeatures = 500;

        private static readonly HashSet<string> ValidGeometryTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Polygon",
            "LineString",
        };

        /// <inheritdoc />
        public async Task<AreaImportParseResult> ParseFileAsync(
            Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            var result = new AreaImportParseResult();

            try
            {
                var extension = Path.GetExtension(fileName).ToLowerInvariant();

                switch (extension)
                {
                    case ".geojson":
                    case ".json":
                        await ParseGeoJsonAsync(fileStream, result, cancellationToken);
                        break;
                    case ".kml":
                        ParseKml(fileStream, result);
                        break;
                    case ".kmz":
                        ParseKmz(fileStream, result);
                        break;
                    case ".zip":
                        ParseShapefile(fileStream, result);
                        break;
                    default:
                        result.Error = $"Unsupported file format: {extension}. Accepted formats: .geojson, .json, .kml, .kmz, .zip (Shapefile)";
                        return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse area import file: {FileName}", fileName);
                result.Error = $"Failed to parse file: {ex.Message}";
                return result;
            }

            // Compute summary
            result.TotalFeatures = result.Features.Count;
            result.ValidFeatures = result.Features.Count(f => f.IsValid);

            // Collect all unique property keys
            var allKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var feature in result.Features)
            {
                foreach (var key in feature.Properties.Keys)
                {
                    allKeys.Add(key);
                }
            }

            result.PropertyKeys = allKeys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();

            return result;
        }

        private async Task ParseGeoJsonAsync(Stream stream, AreaImportParseResult result,
            CancellationToken cancellationToken)
        {
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = doc.RootElement;

            var type = root.GetProperty("type").GetString();

            if (string.Equals(type, "FeatureCollection", StringComparison.OrdinalIgnoreCase))
            {
                if (!root.TryGetProperty("features", out var features) ||
                    features.ValueKind != JsonValueKind.Array)
                {
                    result.Error = "GeoJSON FeatureCollection has no 'features' array.";
                    return;
                }

                if (features.GetArrayLength() > MaxFeatures)
                {
                    result.Error =
                        $"File contains {features.GetArrayLength()} features, which exceeds the maximum of {MaxFeatures}. Please split into smaller files.";
                    return;
                }

                foreach (var featureEl in features.EnumerateArray())
                {
                    result.Features.Add(ParseGeoJsonFeature(featureEl, result));
                }
            }
            else if (string.Equals(type, "Feature", StringComparison.OrdinalIgnoreCase))
            {
                result.Features.Add(ParseGeoJsonFeature(root, result));
            }
            else
            {
                result.Error =
                    "GeoJSON file must be a FeatureCollection or Feature. Found type: " + (type ?? "unknown");
            }
        }

        private static AreaImportFeature ParseGeoJsonFeature(JsonElement featureEl, AreaImportParseResult result)
        {
            var feature = new AreaImportFeature();

            // Extract geometry
            if (featureEl.TryGetProperty("geometry", out var geometryEl) &&
                geometryEl.ValueKind == JsonValueKind.Object)
            {
                var geomType = geometryEl.TryGetProperty("type", out var typeEl) ? typeEl.GetString() : null;
                feature.GeometryType = geomType ?? string.Empty;
                feature.GeoJson = geometryEl.GetRawText();

                if (!ValidGeometryTypes.Contains(feature.GeometryType))
                {
                    feature.IsValid = false;
                    feature.ValidationErrors.Add(
                        $"Unsupported geometry type: {feature.GeometryType}. Only Polygon and LineString are supported.");
                    result.Warnings.Add(
                        $"Feature skipped: unsupported geometry type '{feature.GeometryType}'.");
                }
            }
            else
            {
                feature.IsValid = false;
                feature.ValidationErrors.Add("Feature has no geometry.");
                result.Warnings.Add("Feature skipped: no geometry found.");
            }

            // Extract properties
            if (featureEl.TryGetProperty("properties", out var propsEl) &&
                propsEl.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in propsEl.EnumerateObject())
                {
                    var value = prop.Value.ValueKind switch
                    {
                        JsonValueKind.String => prop.Value.GetString() ?? string.Empty,
                        JsonValueKind.Number => prop.Value.GetRawText(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        JsonValueKind.Null => string.Empty,
                        _ => prop.Value.GetRawText(),
                    };
                    feature.Properties[prop.Name] = value;
                }
            }

            return feature;
        }

        private static void ParseKml(Stream stream, AreaImportParseResult result)
        {
            var doc = XDocument.Load(stream);
            var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

            var placemarks = doc.Descendants(ns + "Placemark").ToList();

            if (placemarks.Count > MaxFeatures)
            {
                result.Error =
                    $"File contains {placemarks.Count} placemarks, which exceeds the maximum of {MaxFeatures}. Please split into smaller files.";
                return;
            }

            foreach (var placemark in placemarks)
            {
                var feature = new AreaImportFeature();

                // Extract name and description as properties
                var name = placemark.Element(ns + "name")?.Value;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    feature.Properties["name"] = name.Trim();
                }

                var description = placemark.Element(ns + "description")?.Value;
                if (!string.IsNullOrWhiteSpace(description))
                {
                    feature.Properties["description"] = description.Trim();
                }

                // Extract ExtendedData
                var extData = placemark.Element(ns + "ExtendedData");
                if (extData != null)
                {
                    foreach (var data in extData.Elements(ns + "Data"))
                    {
                        var dataName = data.Attribute("name")?.Value;
                        var dataValue = data.Element(ns + "value")?.Value;
                        if (!string.IsNullOrWhiteSpace(dataName))
                        {
                            feature.Properties[dataName] = dataValue?.Trim() ?? string.Empty;
                        }
                    }

                    // Also handle SimpleData inside SchemaData
                    foreach (var schemaData in extData.Elements(ns + "SchemaData"))
                    {
                        foreach (var simpleData in schemaData.Elements(ns + "SimpleData"))
                        {
                            var sdName = simpleData.Attribute("name")?.Value;
                            if (!string.IsNullOrWhiteSpace(sdName))
                            {
                                feature.Properties[sdName] = simpleData.Value?.Trim() ?? string.Empty;
                            }
                        }
                    }
                }

                // Extract geometry
                var polygon = placemark.Descendants(ns + "Polygon").FirstOrDefault();
                var lineString = placemark.Descendants(ns + "LineString").FirstOrDefault();

                if (polygon != null)
                {
                    var coords = ParseKmlCoordinates(
                        polygon.Descendants(ns + "coordinates").FirstOrDefault()?.Value);
                    if (coords.Count >= 3)
                    {
                        feature.GeometryType = "Polygon";
                        feature.GeoJson = BuildPolygonGeoJson(coords);
                    }
                    else
                    {
                        feature.IsValid = false;
                        feature.ValidationErrors.Add("Polygon has fewer than 3 coordinates.");
                        result.Warnings.Add(
                            $"Feature '{name ?? "unnamed"}' skipped: polygon has fewer than 3 coordinates.");
                    }
                }
                else if (lineString != null)
                {
                    var coords = ParseKmlCoordinates(
                        lineString.Descendants(ns + "coordinates").FirstOrDefault()?.Value);
                    if (coords.Count >= 2)
                    {
                        feature.GeometryType = "LineString";
                        feature.GeoJson = BuildLineStringGeoJson(coords);
                    }
                    else
                    {
                        feature.IsValid = false;
                        feature.ValidationErrors.Add("LineString has fewer than 2 coordinates.");
                        result.Warnings.Add(
                            $"Feature '{name ?? "unnamed"}' skipped: line has fewer than 2 coordinates.");
                    }
                }
                else
                {
                    // Check for Point — we skip these with a warning
                    var point = placemark.Descendants(ns + "Point").FirstOrDefault();
                    if (point != null)
                    {
                        feature.IsValid = false;
                        feature.ValidationErrors.Add(
                            "Point geometry is not supported. Only Polygon and LineString are accepted.");
                        result.Warnings.Add(
                            $"Feature '{name ?? "unnamed"}' skipped: Point geometry is not supported.");
                    }
                    else
                    {
                        feature.IsValid = false;
                        feature.ValidationErrors.Add("No supported geometry found.");
                        result.Warnings.Add($"Feature '{name ?? "unnamed"}' skipped: no geometry found.");
                    }
                }

                result.Features.Add(feature);
            }
        }

        private static void ParseKmz(Stream stream, AreaImportParseResult result)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var kmlEntry = archive.Entries
                .FirstOrDefault(e => e.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase));

            if (kmlEntry == null)
            {
                result.Error = "KMZ file does not contain a .kml file.";
                return;
            }

            using var kmlStream = kmlEntry.Open();
            ParseKml(kmlStream, result);
        }

        private void ParseShapefile(Stream stream, AreaImportParseResult result)
        {
            // Extract zip to a temp directory
            var tempDir = Path.Combine(Path.GetTempPath(), "trashmob-shp-" + Guid.NewGuid().ToString("N"));

            try
            {
                Directory.CreateDirectory(tempDir);

                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(tempDir);
                }

                // Find the .shp file
                var shpFiles = Directory.GetFiles(tempDir, "*.shp", SearchOption.AllDirectories);
                if (shpFiles.Length == 0)
                {
                    result.Error =
                        "ZIP file does not contain a .shp file. Please upload a ZIP containing .shp, .dbf, .shx, and .prj files.";
                    return;
                }

                var shpPath = shpFiles[0];
                var features = Shapefile.ReadAllFeatures(shpPath);

                if (features.Length > MaxFeatures)
                {
                    result.Error =
                        $"Shapefile contains {features.Length} features, which exceeds the maximum of {MaxFeatures}. Please split into smaller files.";
                    return;
                }

                foreach (var ntsFeature in features)
                {
                    var feature = new AreaImportFeature();

                    // Extract attributes as properties
                    if (ntsFeature.Attributes != null)
                    {
                        foreach (var attrName in ntsFeature.Attributes.GetNames())
                        {
                            var val = ntsFeature.Attributes[attrName];
                            feature.Properties[attrName] = val?.ToString() ?? string.Empty;
                        }
                    }

                    // Convert geometry to GeoJSON
                    if (ntsFeature.Geometry != null)
                    {
                        var geomType = ntsFeature.Geometry.GeometryType;

                        if (string.Equals(geomType, "Polygon", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(geomType, "MultiPolygon", StringComparison.OrdinalIgnoreCase))
                        {
                            // For MultiPolygon, use the first polygon
                            var geom = ntsFeature.Geometry;
                            if (string.Equals(geomType, "MultiPolygon", StringComparison.OrdinalIgnoreCase) &&
                                geom.NumGeometries > 0)
                            {
                                geom = geom.GetGeometryN(0);
                                if (ntsFeature.Geometry.NumGeometries > 1)
                                {
                                    result.Warnings.Add(
                                        $"Feature '{feature.Properties.GetValueOrDefault("name", "unnamed")}': MultiPolygon converted to single Polygon (first part used).");
                                }
                            }

                            var coords = geom.Coordinates
                                .Select(c => new[] { c.X, c.Y })
                                .ToList();

                            if (coords.Count >= 3)
                            {
                                feature.GeometryType = "Polygon";
                                feature.GeoJson = BuildPolygonGeoJson(coords);
                            }
                            else
                            {
                                feature.IsValid = false;
                                feature.ValidationErrors.Add("Polygon has fewer than 3 coordinates.");
                            }
                        }
                        else if (string.Equals(geomType, "LineString", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(geomType, "MultiLineString", StringComparison.OrdinalIgnoreCase))
                        {
                            var geom = ntsFeature.Geometry;
                            if (string.Equals(geomType, "MultiLineString", StringComparison.OrdinalIgnoreCase) &&
                                geom.NumGeometries > 0)
                            {
                                geom = geom.GetGeometryN(0);
                                if (ntsFeature.Geometry.NumGeometries > 1)
                                {
                                    result.Warnings.Add(
                                        $"Feature '{feature.Properties.GetValueOrDefault("name", "unnamed")}': MultiLineString converted to single LineString (first part used).");
                                }
                            }

                            var coords = geom.Coordinates
                                .Select(c => new[] { c.X, c.Y })
                                .ToList();

                            if (coords.Count >= 2)
                            {
                                feature.GeometryType = "LineString";
                                feature.GeoJson = BuildLineStringGeoJson(coords);
                            }
                            else
                            {
                                feature.IsValid = false;
                                feature.ValidationErrors.Add("LineString has fewer than 2 coordinates.");
                            }
                        }
                        else
                        {
                            feature.IsValid = false;
                            feature.ValidationErrors.Add(
                                $"Unsupported geometry type: {geomType}. Only Polygon and LineString are supported.");
                            result.Warnings.Add($"Feature skipped: unsupported geometry type '{geomType}'.");
                        }
                    }
                    else
                    {
                        feature.IsValid = false;
                        feature.ValidationErrors.Add("Feature has no geometry.");
                    }

                    result.Features.Add(feature);
                }
            }
            finally
            {
                // Clean up temp directory
                try
                {
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to clean up temp directory: {TempDir}", tempDir);
                }
            }
        }

        // ============================================================================
        // KML coordinate parsing helpers
        // ============================================================================

        /// <summary>
        /// Parses KML coordinate string (lng,lat,alt whitespace-separated) into [lng, lat] pairs.
        /// </summary>
        private static List<double[]> ParseKmlCoordinates(string? coordString)
        {
            if (string.IsNullOrWhiteSpace(coordString))
            {
                return new List<double[]>();
            }

            var coords = new List<double[]>();
            var tuples = coordString.Trim().Split(new[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var tuple in tuples)
            {
                var parts = tuple.Split(',');
                if (parts.Length >= 2 &&
                    double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var lng) &&
                    double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var lat))
                {
                    coords.Add(new[] { lng, lat });
                }
            }

            return coords;
        }

        // ============================================================================
        // GeoJSON construction helpers
        // ============================================================================

        private static string BuildPolygonGeoJson(List<double[]> coords)
        {
            // Ensure ring is closed
            if (coords.Count > 0 &&
                (Math.Abs(coords[0][0] - coords[^1][0]) > 1e-10 ||
                 Math.Abs(coords[0][1] - coords[^1][1]) > 1e-10))
            {
                coords.Add(new[] { coords[0][0], coords[0][1] });
            }

            var coordArray = string.Join(",",
                coords.Select(c =>
                    $"[{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}]"));

            return $"{{\"type\":\"Polygon\",\"coordinates\":[[{coordArray}]]}}";
        }

        private static string BuildLineStringGeoJson(List<double[]> coords)
        {
            var coordArray = string.Join(",",
                coords.Select(c =>
                    $"[{c[0].ToString(CultureInfo.InvariantCulture)},{c[1].ToString(CultureInfo.InvariantCulture)}]"));

            return $"{{\"type\":\"LineString\",\"coordinates\":[{coordArray}]}}";
        }
    }
}
