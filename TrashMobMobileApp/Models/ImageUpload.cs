namespace TrashMobMobileApp.Models
{
    using Microsoft.AspNetCore.Http;
    using System;

    internal class ImageUpload
    {
        public IFormFile FormFile { get; set; }

        public ImageTypeEnum ImageType { get; set; }

        public Guid ParentId { get; set; }
    }

    public enum ImageTypeEnum
    {
        Before = 1,
        After = 2,
        Pickup = 3,
    }
}
