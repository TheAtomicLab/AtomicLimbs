using Limbs.Web.Helpers;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.BlobStorage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.IO;

namespace Limbs.Web.Logic.Services
{
    public class UserFilesInAzureStorage : IUserFiles
    {
        private readonly CloudBlobContainer _userFilesContainer;

        public UserFilesInAzureStorage()
        {
            var storageAccount = AzureStorageAccount.DefaultAccount;
            var blobClient = storageAccount.CreateCloudBlobClient();
            _userFilesContainer = blobClient.GetContainerReference(AzureStorageContainer.UserFiles);
        }

        public Uri UploadOrderFile(Stream file, string name)
        {
            Image image = file.GenerateFromStreamAndResize(new Size(1000, 1000));
            byte[] imageBytes = image.ToByteArray();

            var blockBlob = _userFilesContainer.GetBlockBlobReference(name);
            blockBlob.UploadFromByteArray(imageBytes, 0, imageBytes.Length);

            return blockBlob.StorageUri.PrimaryUri;
        }
    }
}