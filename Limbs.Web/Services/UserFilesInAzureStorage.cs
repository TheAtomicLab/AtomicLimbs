using System;
using System.IO;
using Limbs.Web.Repositories.Interfaces;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Threading;
using Google.Apis.Util.Store;
using System.Collections.Generic;
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

            /*
            if (file != null && file.Length > 0)
            {
                try
                {                   
                    //define credential
                    UserCredential credential = GetUserCredential();

                    //define service 
                    DriveService service = GetDriveService(credential);

                    //ListFiles
                    //ListFiles(service);   

                    //uploadFile
                    UploadFileDrive(path, service, name);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
            }
            */
        }
        
        //-----------------------------Google drive api------------------------//

        private static void ListFiles(DriveService service)
        {
            //define files
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    //muestro atributos
                }
            }
            else
            {
                //no hay archivos
            }
        }


        private static void UploadFileDrive(string path, DriveService service, string name)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path,
                            System.IO.FileMode.Open))
            {
                request = service.Files.Create(
                fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
        }

        private static UserCredential GetUserCredential()
        {
            string[] Scopes = { DriveService.Scope.DriveFile };

            //var client_json = Server.MapPath("/Scripts/client_secret.json"); --se usa con el choclo de abajo porque GetUserCredential es static--
            var client_json = System.Web.HttpContext.Current.Server.MapPath("/Scripts/client_secret.json");

            using (var stream =
                new FileStream(client_json, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }

        private static DriveService GetDriveService(UserCredential credential)
        {
            var applicationName = "Limbs";
            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }
        //--------------------------------------------------------------------//

    }
}