using Limbs.QueueConsumers;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.BlobStorage;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.SqlServer.Dac;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Limbs.Worker
{
    public class Functions
    {
        public static void ProcessAppExceptions([QueueTrigger(nameof(AppException))] string queueMessage, DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt, int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount, TextWriter logger)
        {
            var queueM = MessageQueue<AppException>.GenerateQueueMessage(queueMessage, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount, queueTrigger, cloudStorageAccount);

            new AppExceptionsSaver().ProcessMessages(queueM);

            logger.WriteLine($"AppExceptionsSaver: {queueM.Data.CustomMessage}");
        }

        public static void MailsMessagesSender([QueueTrigger(nameof(MailMessage))] string queueMessage, DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt, int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount, TextWriter logger)
        {
            var queueM = MessageQueue<MailMessage>.GenerateQueueMessage(queueMessage, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount, queueTrigger, cloudStorageAccount);

            new MailsMessagesSender().ProcessMessages(queueM);

            logger.WriteLine($"MailsMessagesSender: {queueM.Data.Subject} (f: {queueM.Data.From} |t: {queueM.Data.To})");
        }

        public static void ProductGeneratorResult([QueueTrigger(nameof(OrderProductGeneratorResult))] string queueMessage, DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt, int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount, TextWriter logger)
        {
            var queueM = MessageQueue<OrderProductGeneratorResult>.GenerateQueueMessage(queueMessage, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount, queueTrigger, cloudStorageAccount);

            new ProductGeneratorResult().ProcessMessages(queueM);

            logger.WriteLine($"ProductGeneratorResult: {queueM.Data.OrderId} (f: {queueM.Data.FileUrl})");
        }

        public static void BackupBDTrigger([TimerTrigger("0 59 23 * * 0", RunOnStartup = true)]TimerInfo myTimer, TextWriter log)
        {
            try
            {
                CloudStorageAccount storageAccount = AzureStorageAccount.DefaultAccount;
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(AzureStorageContainer.BackupsBD);
                blobContainer.CreateIfNotExists();

                BlobContainerPermissions permissions = blobContainer.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                blobContainer.SetPermissions(permissions);

                string storageName = $"backup_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.bacpac";
                CloudBlockBlob backupFile = blobContainer.GetBlockBlobReference(storageName);

                string cnn = ConfigurationManager.ConnectionStrings["Limbs"].ConnectionString;
                string dbName = null;

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cnn);
                if (builder != null && builder.ContainsKey("Database"))
                {
                    dbName = builder["Database"] as string;

                    if (string.IsNullOrEmpty(dbName))
                        dbName = builder["Initial Catalog"] as string;
                }

                string tempFile = $"{Path.GetTempPath()}{backupFile.Name}";

                DacServices services = new DacServices(cnn);
                services.ExportBacpac(tempFile, dbName);

                backupFile.UploadFromFile(tempFile);

                log.WriteLine($"URL PRIMARIA BLOB: {backupFile.StorageUri.PrimaryUri}");
                log.WriteLine($"URL PRIMARIA BLOB: {backupFile.StorageUri.SecondaryUri}");

                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                log.WriteLine($"OCURRIO UN ERROR: {ex.Message}");
            }
        }
    }
}
