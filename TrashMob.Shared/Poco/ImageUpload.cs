namespace TrashMob.Shared.Poco
{
    using System;
    using Microsoft.AspNetCore.Http;
    using TrashMob.Models;

    public class ImageUpload
    {
        public IFormFile FormFile { get; set; }

        public ImageTypeEnum ImageType { get; set; }

        public Guid ParentId { get; set; }
    }
}