namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Areas;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing adoptable areas within a community.
    /// </summary>
    [Route("api/communities/{partnerId}/areas")]
    public class AdoptableAreasController(
        IAdoptableAreaManager areaManager,
        IKeyedManager<Partner> partnerManager,
        IAreaSuggestionService areaSuggestionService,
        IAreaFileParser areaFileParser,
        IAreaGenerationBatchManager batchManager)
        : SecureController
    {

        /// <summary>
        /// Gets all adoptable areas for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AdoptableArea>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAreas(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            var areas = await areaManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(areas);
        }

        /// <summary>
        /// Gets all available adoptable areas for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<AdoptableArea>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvailableAreas(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            var areas = await areaManager.GetAvailableByCommunityAsync(partnerId, cancellationToken);
            return Ok(areas);
        }

        /// <summary>
        /// Gets a specific adoptable area.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="areaId">The area ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{areaId}")]
        [ProducesResponseType(typeof(AdoptableArea), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArea(Guid partnerId, Guid areaId, CancellationToken cancellationToken)
        {
            var area = await areaManager.GetAsync(areaId, cancellationToken);
            if (area is null || area.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(area);
        }

        /// <summary>
        /// Checks if an area name is available within a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="name">The area name to check.</param>
        /// <param name="excludeAreaId">Optional area ID to exclude (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("check-name")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckAreaName(
            Guid partnerId,
            [FromQuery] string name,
            [FromQuery] Guid? excludeAreaId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(false);
            }

            var isAvailable = await areaManager.IsNameAvailableAsync(partnerId, name, excludeAreaId, cancellationToken);
            return Ok(isAvailable);
        }

        /// <summary>
        /// Uses AI to suggest an area geometry from a natural language description.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="request">The suggestion request containing the area description.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("suggest")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaSuggestionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SuggestArea(
            Guid partnerId,
            [FromBody] AreaSuggestionRequest request,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                return BadRequest("A description is required.");
            }

            var result = await areaSuggestionService.SuggestAreaAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new adoptable area. Only community admins can create areas.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="area">The area to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AdoptableArea), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateArea(
            Guid partnerId,
            [FromBody] AdoptableArea area,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            // Check authorization
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            // Check if area name is available
            var isAvailable = await areaManager.IsNameAvailableAsync(partnerId, area.Name, cancellationToken: cancellationToken);
            if (!isAvailable)
            {
                return BadRequest("An area with this name already exists in this community.");
            }

            // Set required fields
            area.Id = Guid.NewGuid();
            area.PartnerId = partnerId;
            area.IsActive = true;
            area.CreatedByUserId = UserId;
            area.CreatedDate = DateTimeOffset.UtcNow;
            area.LastUpdatedByUserId = UserId;
            area.LastUpdatedDate = DateTimeOffset.UtcNow;

            var createdArea = await areaManager.AddAsync(area, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetArea), new { partnerId, areaId = createdArea.Id }, createdArea);
        }

        /// <summary>
        /// Updates an adoptable area. Only community admins can update areas.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="areaId">The area ID.</param>
        /// <param name="area">The updated area data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{areaId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AdoptableArea), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateArea(
            Guid partnerId,
            Guid areaId,
            [FromBody] AdoptableArea area,
            CancellationToken cancellationToken)
        {
            if (areaId != area.Id)
            {
                return BadRequest("Area ID mismatch.");
            }

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            // Check authorization
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existingArea = await areaManager.GetAsync(areaId, cancellationToken);
            if (existingArea is null || existingArea.PartnerId != partnerId)
            {
                return NotFound();
            }

            // Check if name is being changed and is available
            if (!string.Equals(existingArea.Name, area.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isAvailable = await areaManager.IsNameAvailableAsync(partnerId, area.Name, areaId, cancellationToken);
                if (!isAvailable)
                {
                    return BadRequest("An area with this name already exists in this community.");
                }
            }

            // Update fields
            existingArea.Name = area.Name;
            existingArea.Description = area.Description;
            existingArea.AreaType = area.AreaType;
            existingArea.Status = area.Status;
            existingArea.GeoJson = area.GeoJson;
            existingArea.StartLatitude = area.StartLatitude;
            existingArea.StartLongitude = area.StartLongitude;
            existingArea.EndLatitude = area.EndLatitude;
            existingArea.EndLongitude = area.EndLongitude;
            existingArea.CleanupFrequencyDays = area.CleanupFrequencyDays;
            existingArea.MinEventsPerYear = area.MinEventsPerYear;
            existingArea.SafetyRequirements = area.SafetyRequirements;
            existingArea.AllowCoAdoption = area.AllowCoAdoption;
            existingArea.IsActive = area.IsActive;
            existingArea.LastUpdatedByUserId = UserId;
            existingArea.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedArea = await areaManager.UpdateAsync(existingArea, UserId, cancellationToken);
            return Ok(updatedArea);
        }

        /// <summary>
        /// Parses an uploaded area file (GeoJSON, KML, KMZ, or Shapefile) and returns normalized features.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="file">The uploaded file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("import/parse")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [RequestSizeLimit(10 * 1024 * 1024)]
        [ProducesResponseType(typeof(AreaImportParseResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ParseImportFile(
            Guid partnerId,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (file is null || file.Length == 0)
            {
                return BadRequest("A file is required.");
            }

            var allowedExtensions = new[] { ".geojson", ".json", ".kml", ".kmz", ".zip" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(
                    $"Unsupported file format: {extension}. Accepted formats: .geojson, .json, .kml, .kmz, .zip (Shapefile)");
            }

            using var stream = file.OpenReadStream();
            var result = await areaFileParser.ParseFileAsync(stream, file.FileName, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Bulk imports areas into a community. Only community admins can import areas.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="areas">The areas to import.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("import")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaBulkImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkImportAreas(
            Guid partnerId,
            [FromBody] List<AdoptableArea> areas,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (areas is null || areas.Count == 0)
            {
                return BadRequest("At least one area is required.");
            }

            if (areas.Count > 500)
            {
                return BadRequest("Maximum 500 areas per import. Please split into smaller batches.");
            }

            var result = await areaManager.BulkCreateAsync(partnerId, UserId, areas, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Exports all adoptable areas for a community as GeoJSON or KML.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="format">The export format: geojson or kml.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("export")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportAreas(
            Guid partnerId,
            [FromQuery] string format,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var areas = await areaManager.GetByCommunityAsync(partnerId, cancellationToken);
            var areaList = areas.ToList();

            var safeName = partner.Name.Replace(" ", "_", StringComparison.Ordinal);
            var dateSuffix = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            if (string.Equals(format, "kml", StringComparison.OrdinalIgnoreCase))
            {
                var kml = BuildKml(partner.Name, areaList);
                var bytes = Encoding.UTF8.GetBytes(kml);
                return File(bytes, "application/vnd.google-earth.kml+xml", $"{safeName}_Areas_{dateSuffix}.kml");
            }

            // Default to GeoJSON
            var geoJson = BuildGeoJson(areaList);
            var geoJsonBytes = Encoding.UTF8.GetBytes(geoJson);
            return File(geoJsonBytes, "application/geo+json", $"{safeName}_Areas_{dateSuffix}.geojson");
        }

        /// <summary>
        /// Deletes (deactivates) an adoptable area. Only community admins can delete areas.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="areaId">The area ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{areaId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteArea(
            Guid partnerId,
            Guid areaId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            // Check authorization
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existingArea = await areaManager.GetAsync(areaId, cancellationToken);
            if (existingArea is null || existingArea.PartnerId != partnerId)
            {
                return NotFound();
            }

            // Soft delete
            existingArea.IsActive = false;
            existingArea.LastUpdatedByUserId = UserId;
            existingArea.LastUpdatedDate = DateTimeOffset.UtcNow;

            await areaManager.UpdateAsync(existingArea, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Clears all adoptable areas, staged areas, and generation batches for a community.
        /// Adoptable areas are soft-deleted (deactivated). Generation batches and staged areas are hard-deleted.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("clear-all")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaBulkClearResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearAll(
            Guid partnerId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var areasDeactivated = await areaManager.ClearAllByPartnerAsync(partnerId, UserId, cancellationToken);
            var (batchesDeleted, stagedAreasDeleted) = await batchManager.DeleteAllByPartnerAsync(partnerId, cancellationToken);

            TrackEvent("ClearAllAreas");

            return Ok(new AreaBulkClearResult
            {
                AreasDeactivated = areasDeactivated,
                StagedAreasDeleted = stagedAreasDeleted,
                BatchesDeleted = batchesDeleted,
            });
        }

        #region Export Helpers

        private static readonly JsonSerializerOptions GeoJsonSerializerOptions = new() { WriteIndented = true };

        private static string BuildGeoJson(List<AdoptableArea> areas)
        {
            var features = new List<object>();

            foreach (var area in areas)
            {
                object geometry = null;
                if (!string.IsNullOrWhiteSpace(area.GeoJson))
                {
                    try
                    {
                        geometry = JsonSerializer.Deserialize<JsonElement>(area.GeoJson);
                    }
                    catch (JsonException)
                    {
                        // Skip malformed geometry
                    }
                }

                features.Add(new
                {
                    type = "Feature",
                    geometry,
                    properties = new
                    {
                        name = area.Name,
                        description = area.Description ?? "",
                        areaType = area.AreaType,
                        status = area.Status,
                        cleanupFrequencyDays = area.CleanupFrequencyDays,
                        minEventsPerYear = area.MinEventsPerYear,
                        safetyRequirements = area.SafetyRequirements ?? "",
                        allowCoAdoption = area.AllowCoAdoption,
                    },
                });
            }

            var featureCollection = new
            {
                type = "FeatureCollection",
                features,
            };

            return JsonSerializer.Serialize(featureCollection, GeoJsonSerializerOptions);
        }

        private static string BuildKml(string communityName, List<AdoptableArea> areas)
        {
            XNamespace kmlNs = "http://www.opengis.net/kml/2.2";

            var placemarks = new List<XElement>();

            foreach (var area in areas)
            {
                var placemark = new XElement(kmlNs + "Placemark",
                    new XElement(kmlNs + "name", area.Name),
                    new XElement(kmlNs + "description", area.Description ?? ""),
                    new XElement(kmlNs + "ExtendedData",
                        KmlDataElement(kmlNs, "areaType", area.AreaType),
                        KmlDataElement(kmlNs, "status", area.Status),
                        KmlDataElement(kmlNs, "cleanupFrequencyDays", area.CleanupFrequencyDays.ToString(CultureInfo.InvariantCulture)),
                        KmlDataElement(kmlNs, "minEventsPerYear", area.MinEventsPerYear.ToString(CultureInfo.InvariantCulture)),
                        KmlDataElement(kmlNs, "safetyRequirements", area.SafetyRequirements ?? ""),
                        KmlDataElement(kmlNs, "allowCoAdoption", area.AllowCoAdoption.ToString())));

                var geometryElement = GeoJsonToKmlGeometry(kmlNs, area.GeoJson);
                if (geometryElement != null)
                {
                    placemark.Add(geometryElement);
                }

                placemarks.Add(placemark);
            }

            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(kmlNs + "kml",
                    new XElement(kmlNs + "Document",
                        new XElement(kmlNs + "name", $"{communityName} Areas"),
                        placemarks)));

            return doc.Declaration + Environment.NewLine + doc;
        }

        private static XElement KmlDataElement(XNamespace kmlNs, string name, string value)
        {
            return new XElement(kmlNs + "Data",
                new XAttribute("name", name),
                new XElement(kmlNs + "value", value));
        }

        private static XElement GeoJsonToKmlGeometry(XNamespace kmlNs, string geoJson)
        {
            if (string.IsNullOrWhiteSpace(geoJson))
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(geoJson);
                var root = doc.RootElement;
                var type = root.GetProperty("type").GetString();

                if (type == "Polygon")
                {
                    var coords = root.GetProperty("coordinates");
                    var outerRing = coords[0];
                    return new XElement(kmlNs + "Polygon",
                        new XElement(kmlNs + "outerBoundaryIs",
                            new XElement(kmlNs + "LinearRing",
                                new XElement(kmlNs + "coordinates", CoordinateArrayToKml(outerRing)))));
                }

                if (type == "LineString")
                {
                    var coords = root.GetProperty("coordinates");
                    return new XElement(kmlNs + "LineString",
                        new XElement(kmlNs + "coordinates", CoordinateArrayToKml(coords)));
                }

                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static string CoordinateArrayToKml(JsonElement coordinates)
        {
            var sb = new StringBuilder();
            foreach (var coord in coordinates.EnumerateArray())
            {
                var lon = coord[0].GetDouble().ToString(CultureInfo.InvariantCulture);
                var lat = coord[1].GetDouble().ToString(CultureInfo.InvariantCulture);
                var alt = coord.GetArrayLength() > 2
                    ? coord[2].GetDouble().ToString(CultureInfo.InvariantCulture)
                    : "0";
                sb.Append($"{lon},{lat},{alt} ");
            }

            return sb.ToString().TrimEnd();
        }

        #endregion
    }
}
