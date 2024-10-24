﻿namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    public interface IImageManager
    {
        public Task UploadImage(ImageUpload imageUpload);

        public Task<string> GetImageUrlAsync(Guid parentId, ImageTypeEnum imageType, ImageSizeEnum imageSize, CancellationToken cancellationToken);

        public Task<bool> DeleteImage(Guid parentId, ImageTypeEnum imageType);
    }
}