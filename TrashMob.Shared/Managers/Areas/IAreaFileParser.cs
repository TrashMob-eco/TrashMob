#nullable enable

namespace TrashMob.Shared.Managers.Areas
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Parses uploaded area files (GeoJSON, KML, KMZ, Shapefile) into normalized features.
    /// </summary>
    public interface IAreaFileParser
    {
        /// <summary>
        /// Parses an uploaded file and returns normalized features with properties and geometry.
        /// </summary>
        /// <param name="fileStream">The uploaded file stream.</param>
        /// <param name="fileName">The original file name (used for format detection).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A parse result containing features, property keys, and any warnings or errors.</returns>
        Task<AreaImportParseResult> ParseFileAsync(
            Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    }
}
