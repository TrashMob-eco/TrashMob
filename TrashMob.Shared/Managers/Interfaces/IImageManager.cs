namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing images in blob storage.
    /// </summary>
    public interface IImageManager
    {
        /// <summary>
        /// Uploads an image to blob storage.
        /// </summary>
        /// <param name="imageUpload">The image upload request containing image data.</param>
        public Task UploadImage(ImageUpload imageUpload);

        /// <summary>
        /// Gets the URL for an image in blob storage.
        /// </summary>
        /// <param name="parentId">The parent entity ID.</param>
        /// <param name="imageType">The type of image.</param>
        /// <param name="imageSize">The desired image size.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The image URL or null if not found.</returns>
        public Task<string> GetImageUrlAsync(Guid parentId, ImageTypeEnum imageType, ImageSizeEnum imageSize, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an image from blob storage.
        /// </summary>
        /// <param name="parentId">The parent entity ID.</param>
        /// <param name="imageType">The type of image to delete.</param>
        /// <returns>True if the image was deleted, false otherwise.</returns>
        public Task<bool> DeleteImage(Guid parentId, ImageTypeEnum imageType);
    }
}