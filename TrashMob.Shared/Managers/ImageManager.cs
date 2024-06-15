namespace TrashMob.Shared.Managers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Logging;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    internal class ImageManager(ILogger<ImageManager> logger, BlobServiceClient blobServiceClient)
        : IImageManager
    {
        public async Task<string> GetImageUrlAsync(Guid parentId, ImageTypeEnum imageType, ImageSizeEnum imageSize, CancellationToken cancellationToken)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(imageType.ToString().ToLower());

            var fileNameFilter = $"{parentId}-{imageType}-{imageSize}"
                .ToLower();

            var imageUrl = string.Empty;

            // Should only be one image.
            await foreach (var blob in blobContainer.GetBlobsAsync(prefix: fileNameFilter, cancellationToken: cancellationToken))
            {
                imageUrl = $"{blobContainer.Uri}/{blob.Name}";
                break;
            }

            return imageUrl;
        }

        public async Task UploadImage(ImageUpload imageUpload)
        {
            if (imageUpload?.FormFile == null)
            {
                throw new ArgumentNullException(nameof(imageUpload));
            }

            const int thumbnailWidth = 100;
            const int thumbnailHeight = 100;

            const int reducedWidth = 400;
            const int reducedHeight = 400;

            var blobContainer = blobServiceClient.GetBlobContainerClient(imageUpload.ImageType.ToString().ToLower());

            var fileTime = DateTimeOffset.UtcNow.ToString("ddHHmmss");

            logger.LogInformation("ParentId: {ParentId}, ImageType: {ImageType}, File: {FileName}",
                imageUpload.ParentId, imageUpload.ImageType, imageUpload.FormFile?.FileName);

            var fileName = string.Format("{0}-{1}-{2}-{3}{4}", imageUpload.ParentId, imageUpload.ImageType.ToString(),
                ImageSizeEnum.Raw.ToString(), fileTime, Path.GetExtension(imageUpload.FormFile?.FileName)).ToLower();

            // Upload the raw file
            if (imageUpload.FormFile == null)
            {
                throw new ArgumentException("ImageUpload FormFile cannot be null");
            }

            await UploadBlob(imageUpload.FormFile.OpenReadStream(), fileName, blobContainer);

            using var memoryStream = new MemoryStream();
            await imageUpload.FormFile.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            // Create a thumbnail
            using (var image = await Image.LoadAsync(memoryStream))
            {
                var thumbNailFileName = string.Format("{0}-{1}-{2}-{3}.jpg", imageUpload.ParentId,
                    imageUpload.ImageType.ToString(), ImageSizeEnum.Thumb.ToString(), fileTime).ToLower();

                image.Mutate(x => x.Resize(thumbnailWidth, thumbnailHeight));

                using var memoryStreamThumbNail = new MemoryStream();
                await image.SaveAsync(memoryStreamThumbNail, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());

                memoryStreamThumbNail.Position = 0;

                await UploadBlob(memoryStreamThumbNail, thumbNailFileName, blobContainer);
            }

            memoryStream.Position = 0;

            // Create a reduced image
            using (var image = await Image.LoadAsync(memoryStream))
            {
                var reducedFileName = string.Format("{0}-{1}-{2}-{3}.jpg", imageUpload.ParentId,
                    imageUpload.ImageType.ToString(), ImageSizeEnum.Reduced.ToString(), fileTime).ToLower();

                image.Mutate(x => x.Resize(reducedWidth, reducedHeight));

                using var memoryStreamReduced = new MemoryStream();
                await image.SaveAsync(memoryStreamReduced, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());

                memoryStreamReduced.Position = 0;

                await UploadBlob(memoryStreamReduced, reducedFileName, blobContainer);
            }
        }

        public async Task<bool> DeleteImage(Guid parentId, ImageTypeEnum imageType)
        {
            var imageName = await GetImageNameAsync(parentId, imageType);

            if (imageName == null)
            {
                return false;
            }

            var blobContainer = blobServiceClient.GetBlobContainerClient(imageType.ToString().ToLower());
            var blobClient = blobContainer.GetBlobClient(imageName);

            return await blobClient.DeleteIfExistsAsync();
        }

        private async Task UploadBlob(Stream stream, string fileName, BlobContainerClient blobContainer)
        {
            logger.LogInformation("Creating blob: {BlobName}, length: {Length}", fileName, stream.Length);

            var blobClient = blobContainer.GetBlobClient(fileName);

            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "image/jpeg" });
        }

        private async Task<string> GetImageNameAsync(Guid parentId, ImageTypeEnum imageType)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(imageType.ToString().ToLower());

            var fileNameFilter = $"{parentId}-{imageType}".ToLower();

            var imageName = string.Empty;

            // For now, only show the first image, since there should only be one for Pickups.
            await foreach (var blob in blobContainer.GetBlobsAsync(prefix: fileNameFilter))
            {
                imageName = blob.Name;
                break;
            }

            return imageName;
        }
    }
}