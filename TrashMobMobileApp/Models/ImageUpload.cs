namespace TrashMobMobileApp.Models
{
    using Microsoft.AspNetCore.Http;
    using System;

    internal class ImageUpload
    {
        public const string After = "After";
        public const string Before = "Before";
        public const string Pickup = "Pickup";

        public IFormFile FormFile { get; set; }

        public string ImageType { get; set; }

        public Guid ParentId { get; set; }
    }
}
