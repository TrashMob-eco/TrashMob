namespace TrashMob.Shared.Poco
{
    using System;
    using Microsoft.AspNetCore.Http;
    using TrashMob.Models;

    /// <summary>
    /// Represents an image upload request with file data and metadata.
    /// </summary>
    public class ImageUpload
    {
        /// <summary>
        /// Gets or sets the uploaded image file.
        /// </summary>
        public IFormFile FormFile { get; set; }

        /// <summary>
        /// Gets or sets the type of image being uploaded.
        /// </summary>
        public ImageTypeEnum ImageType { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the parent entity the image is associated with.
        /// </summary>
        public Guid ParentId { get; set; }
    }
}