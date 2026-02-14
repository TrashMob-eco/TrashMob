namespace TrashMob.Shared.Managers.Partners
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Sas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Manages partner document file storage in Azure Blob Storage.
    /// </summary>
    public class PartnerDocumentStorageManager(
        ILogger<PartnerDocumentStorageManager> logger,
        BlobServiceClient blobServiceClient)
        : IPartnerDocumentStorageManager
    {
        private const string ContainerName = "partner-documents";

        /// <inheritdoc />
        public async Task<string> UploadDocumentAsync(Guid partnerId, Guid documentId, IFormFile file, CancellationToken cancellationToken)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? string.Empty;
            var blobPath = $"{partnerId}/{documentId}{extension}";
            var blobClient = blobContainer.GetBlobClient(blobPath);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = file.ContentType },
                cancellationToken: cancellationToken);

            logger.LogInformation(
                "Uploaded partner document {DocumentId} for partner {PartnerId} ({Size} bytes)",
                documentId,
                partnerId,
                file.Length);

            return blobPath;
        }

        /// <inheritdoc />
        public async Task<string> GetDownloadUrlAsync(string blobPath, CancellationToken cancellationToken)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);
            var blobClient = blobContainer.GetBlobClient(blobPath);

            // Use User Delegation SAS (required for DefaultAzureCredential / Managed Identity)
            var delegationKey = await blobServiceClient.GetUserDelegationKeyAsync(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddMinutes(15),
                cancellationToken);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = ContainerName,
                BlobName = blobPath,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15),
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasBuilder.ToSasQueryParameters(delegationKey, blobServiceClient.AccountName),
            };

            return sasUri.ToUri().ToString();
        }

        /// <inheritdoc />
        public async Task DeleteDocumentAsync(string blobPath, CancellationToken cancellationToken)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);
            var blobClient = blobContainer.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            logger.LogInformation("Deleted partner document blob {BlobPath}", blobPath);
        }

        /// <inheritdoc />
        public async Task<long> GetPartnerStorageUsageBytesAsync(Guid partnerId, CancellationToken cancellationToken)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(ContainerName);

            // Check if container exists before listing
            if (!await blobContainer.ExistsAsync(cancellationToken))
            {
                return 0;
            }

            long totalBytes = 0;
            var prefix = $"{partnerId}/";

            await foreach (var blob in blobContainer.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken))
            {
                totalBytes += blob.Properties.ContentLength ?? 0;
            }

            return totalBytes;
        }
    }
}
