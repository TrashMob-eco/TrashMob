namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using SixLabors.ImageSharp;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using SixLabors.ImageSharp.Processing;

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
            const int ThumbnailWidth = 100;
            const int ThumbnailHeight = 100;

            const int ReducedWidth = 400;
            const int ReducedHeight = 400;

            var blobContainer = blobServiceClient.GetBlobContainerClient(imageUpload.ImageType.ToString().ToLower());
            var fileTime = DateTimeOffset.UtcNow.ToString("ddHHmmss");

            logger.LogInformation("ParentId: {ParentId}, ImageType: {ImageType}, File: {FileName}", imageUpload.ParentId, imageUpload.ImageType, imageUpload.FormFile?.FileName);
            var fileName = string.Format("{0}-{1}-{2}{3}", imageUpload.ParentId, imageUpload.ImageType.ToString(), fileTime, System.IO.Path.GetExtension(imageUpload.FormFile?.FileName)).ToLower();

            var blobClient = blobContainer.GetBlobClient(fileName);

            // Upload the raw file
            await blobClient.UploadAsync(imageUpload.FormFile.OpenReadStream(), new BlobHttpHeaders { ContentType = imageUpload.FormFile.ContentType });

            using var memoryStream = new System.IO.MemoryStream();
            await imageUpload.FormFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Create a thumbnail
            using (Image image = Image.Load(memoryStream))
            {
                var thumbNailFileName = string.Format("{0}-{1}-{2}-thumb.png", imageUpload.ParentId, imageUpload.ImageType.ToString(), fileTime).ToLower();
                var thumbNailBlobClient = blobContainer.GetBlobClient(thumbNailFileName);

                image.Mutate(x => x.Resize(ThumbnailWidth, ThumbnailHeight));

                using var memoryStreamThumbNail = new System.IO.MemoryStream();
                await image.SaveAsync(memoryStreamThumbNail, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                memoryStreamThumbNail.Position = 0;
                await blobClient.UploadAsync(memoryStreamThumbNail, new BlobHttpHeaders { ContentType = imageUpload.FormFile.ContentType });
            }

            memoryStream.Position = 0;

            // Create a reduced image
            using (Image image = Image.Load(memoryStream))
            {
                var reducedFileName = string.Format("{0}-{1}-{2}-reduced.png", imageUpload.ParentId, imageUpload.ImageType.ToString(), fileTime).ToLower();
                var reducedBlobClient = blobContainer.GetBlobClient(reducedFileName);

                image.Mutate(x => x.Resize(ReducedWidth, ReducedHeight));

                using var memoryStreamReduced = new System.IO.MemoryStream();
                await image.SaveAsync(memoryStreamReduced, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                memoryStreamReduced.Position = 0;
                await blobClient.UploadAsync(memoryStreamReduced, new BlobHttpHeaders { ContentType = imageUpload.FormFile.ContentType });
            }
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
