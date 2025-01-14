﻿namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    public interface IImageManager
    {
        public Task UploadImage(ImageUpload imageUpload);

        public Task<string> GetImageUrlAsync(Guid pickupLocationId, ImageTypeEnum imageType);
    }
}
