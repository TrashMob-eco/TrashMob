namespace TrashMob.Shared.Poco
{
    using Microsoft.AspNetCore.Http;
    using System;
    using TrashMob.Shared;

    public class ImageUpload
    {
        public IFormFile FormFile { get; set; }

        public ImageTypeEnum ImageType { get; set; }

        public Guid ParentId { get; set; }
    }
}
