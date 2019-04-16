using Limbs.Web.Helpers;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.BlobStorage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

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

        public async Task<bool> RemoveImageAsync(string imageId)
        {
            var blob = _userFilesContainer.GetBlockBlobReference(imageId);
            return await blob.DeleteIfExistsAsync();
        }

        public Uri UploadOrderFile(Stream file, string name)
        {
            Image image = file.GenerateFromStreamAndResize(new Size(1280, 720));
            byte[] imageBytes = image.ToByteArray();

            var blockBlob = _userFilesContainer.GetBlockBlobReference(name);
            blockBlob.UploadFromByteArray(imageBytes, 0, imageBytes.Length);

            return blockBlob.StorageUri.PrimaryUri;
        }
    }
}