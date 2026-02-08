namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Defines operations for managing partner document file storage in Azure Blob Storage.
    /// </summary>
    public interface IPartnerDocumentStorageManager
    {
        /// <summary>
        /// Uploads a document file to Azure Blob Storage.
        /// </summary>
        /// <param name="partnerId">The partner identifier.</param>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The blob storage path of the uploaded document.</returns>
        Task<string> UploadDocumentAsync(Guid partnerId, Guid documentId, IFormFile file, CancellationToken cancellationToken);

        /// <summary>
        /// Generates a time-limited SAS URL for downloading a document.
        /// </summary>
        /// <param name="blobPath">The blob storage path.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A SAS URL valid for 15 minutes.</returns>
        Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a document file from Azure Blob Storage.
        /// </summary>
        /// <param name="blobPath">The blob storage path.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task DeleteDocumentAsync(string blobPath, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the total storage usage in bytes for a partner's uploaded documents.
        /// </summary>
        /// <param name="partnerId">The partner identifier.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>Total bytes used by the partner's documents.</returns>
        Task<long> GetPartnerStorageUsageBytesAsync(Guid partnerId, CancellationToken cancellationToken);
    }
}
