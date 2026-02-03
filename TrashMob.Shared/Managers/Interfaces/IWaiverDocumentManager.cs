namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Interface for waiver document generation and storage operations.
    /// </summary>
    public interface IWaiverDocumentManager
    {
        /// <summary>
        /// Generates a PDF document for a signed waiver and stores it in blob storage.
        /// </summary>
        /// <param name="userWaiver">The user waiver record.</param>
        /// <param name="user">The user who signed the waiver.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The URL of the stored PDF document.</returns>
        Task<string> GenerateAndStoreWaiverPdfAsync(
            UserWaiver userWaiver,
            User user,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a PDF document for a signed waiver as a byte array.
        /// </summary>
        /// <param name="userWaiver">The user waiver record.</param>
        /// <param name="user">The user who signed the waiver.</param>
        /// <returns>The PDF document as a byte array.</returns>
        byte[] GenerateWaiverPdf(UserWaiver userWaiver, User user);

        /// <summary>
        /// Retrieves a waiver PDF from blob storage.
        /// </summary>
        /// <param name="documentUrl">The URL of the document.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Stream containing the PDF content.</returns>
        Task<Stream> GetWaiverPdfAsync(string documentUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores an uploaded paper waiver document in blob storage.
        /// </summary>
        /// <param name="userWaiver">The user waiver record.</param>
        /// <param name="fileStream">The file stream to upload.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The URL of the stored document.</returns>
        Task<string> StorePaperWaiverAsync(
            UserWaiver userWaiver,
            Stream fileStream,
            string contentType,
            CancellationToken cancellationToken = default);
    }
}
