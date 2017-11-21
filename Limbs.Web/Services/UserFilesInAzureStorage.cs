using System;
using System.IO;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.BlobStorage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Limbs.Web.Services
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
            var blockBlob = _userFilesContainer.GetBlockBlobReference(name);

            blockBlob.UploadFromStream(file);
            
            return blockBlob.StorageUri.PrimaryUri;
        }
    }
}