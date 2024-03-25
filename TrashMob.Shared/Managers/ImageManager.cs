namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    internal class ImageManager : IImageManager
    {
        private readonly ILogger<ImageManager> logger;
        private readonly BlobServiceClient blobServiceClient;

        public ImageManager(ILogger<ImageManager> logger, BlobServiceClient blobServiceClient) 
        {
            this.logger = logger;
            this.blobServiceClient = blobServiceClient;
        }

        public async Task<string> GetImageUrlAsync(Guid parentId, ImageTypeEnum imageType)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(imageType.ToString().ToLower());

            var fileNameFilter = string.Format("{0}-{1}", parentId, imageType.ToString()).ToLower();

            var imageUrl = string.Empty;

            // For now, only show the first image, since there should only be one for Pickups.
            await foreach (BlobItem blob in blobContainer.GetBlobsAsync(prefix: fileNameFilter))
            {
                imageUrl = $"{blobContainer.Uri}/{blob.Name}";
                break;
            }

            return imageUrl;
        }

        public async Task UploadImage(ImageUpload imageUpload)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(imageUpload.ImageType.ToString().ToLower());

            logger.LogInformation("ParentId: {0}, ImageType: {1}, File: {2}", imageUpload.ParentId, imageUpload.ImageType, imageUpload.FormFile?.FileName);
            var fileName = string.Format("{0}-{1}-{2}{3}", imageUpload.ParentId, imageUpload.ImageType.ToString(), DateTimeOffset.UtcNow.ToString("ddHHmmss"), System.IO.Path.GetExtension(imageUpload.FormFile?.FileName)).ToLower();

            var blobClient = blobContainer.GetBlobClient(fileName);

            await blobClient.UploadAsync(imageUpload.FormFile.OpenReadStream(), new BlobHttpHeaders { ContentType = imageUpload.FormFile.ContentType });
        }

        public async Task<bool> DeleteImage(Guid parentId, ImageTypeEnum imageType)
        {
            var imageName = await GetImageNameAsync(parentId, imageType);

            if(imageName == null)
            {
                return false;
            }

            var blobContainer = blobServiceClient.GetBlobContainerClient(imageType.ToString().ToLower());
            var blobClient = blobContainer.GetBlobClient(imageName);

            return await blobClient.DeleteIfExistsAsync();
        }

        private async Task<string> GetImageNameAsync(Guid parentId, ImageTypeEnum imageType)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(imageType.ToString().ToLower());

            var fileNameFilter = string.Format("{0}-{1}", parentId, imageType.ToString()).ToLower();

            var imageName = string.Empty;

            // For now, only show the first image, since there should only be one for Pickups.
            await foreach (BlobItem blob in blobContainer.GetBlobsAsync(prefix: fileNameFilter))
            {
                imageName = blob.Name;
                break;
            }

            return imageName;
        }
    }
}
