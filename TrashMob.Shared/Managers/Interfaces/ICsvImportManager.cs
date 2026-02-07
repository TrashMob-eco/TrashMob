namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Imports community prospects from a CSV file.
    /// </summary>
    public interface ICsvImportManager
    {
        /// <summary>
        /// Parses and imports community prospects from a CSV stream.
        /// </summary>
        /// <param name="csvStream">The CSV file stream.</param>
        /// <param name="userId">The admin user performing the import.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<CsvImportResult> ImportProspectsAsync(Stream csvStream, Guid userId, CancellationToken cancellationToken = default);
    }
}
