namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Exports all personal data for a user in JSON format, streamed to avoid timeout.
    /// </summary>
    public interface IUserDataExportManager
    {
        /// <summary>
        /// Writes the complete user data export as JSON to the provided stream.
        /// Uses Utf8JsonWriter for streaming output to avoid memory and timeout issues.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to export.</param>
        /// <param name="outputStream">The stream to write JSON output to.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task WriteExportToStreamAsync(Guid userId, Stream outputStream, CancellationToken cancellationToken = default);
    }
}
