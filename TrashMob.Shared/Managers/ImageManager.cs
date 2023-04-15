﻿namespace TrashMob.Shared.Managers
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Microsoft.Extensions.Logging;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;

    internal class ImageManager : IImageManager
    {
        private readonly ILogger<ImageManager> logger;
        private readonly BlobServiceClient blobServiceClient;

        public ImageManager(ILogger<ImageManager> logger, BlobServiceClient blobServiceClient) 
        {
            this.logger = logger;
            this.blobServiceClient = blobServiceClient;
        }

        public async Task UploadImage(ImageUpload imageUpload)
        {
            var blobContainer = blobServiceClient.GetBlobContainerClient(imageUpload.ImageType.ToString().ToLower());

            logger.LogInformation("ParentId: {0}, ImageType: {1}, File: {2}", imageUpload.ParentId, imageUpload.ImageType, imageUpload.FormFile?.FileName);
            var fileName = string.Format("{0}-{1}-{2}{3}", imageUpload.ParentId, imageUpload.ImageType.ToString(), DateTimeOffset.UtcNow.ToString("ddHHmmss"), System.IO.Path.GetExtension(imageUpload.FormFile?.FileName)).ToLower();

            var blobClient = blobContainer.GetBlobClient(fileName);

            await blobClient.UploadAsync(imageUpload.FormFile.OpenReadStream(), new BlobHttpHeaders { ContentType = imageUpload.FormFile.ContentType });
        }
    }
}