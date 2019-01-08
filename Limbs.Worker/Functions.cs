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
using System.IO;

namespace Limbs.Worker
{
    public class Functions
    {
        //public static void ProcessAppExceptions([QueueTrigger(nameof(AppException))] string queueMessage, DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt, int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount, TextWriter logger)
        //{
        //    var queueM = MessageQueue<AppException>.GenerateQueueMessage(queueMessage, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount, queueTrigger, cloudStorageAccount);

        //    new AppExceptionsSaver().ProcessMessages(queueM);

        //    logger.WriteLine($"AppExceptionsSaver: {queueM.Data.CustomMessage}");
        //}

        //public static void MailsMessagesSender([QueueTrigger(nameof(MailMessage))] string queueMessage, DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt, int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount, TextWriter logger)
        //{
        //    var queueM = MessageQueue<MailMessage>.GenerateQueueMessage(queueMessage, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount, queueTrigger, cloudStorageAccount);

        //    new MailsMessagesSender().ProcessMessages(queueM);

        //    logger.WriteLine($"MailsMessagesSender: {queueM.Data.Subject} (f: {queueM.Data.From} |t: {queueM.Data.To})");
        //}

        //public static void ProductGeneratorResult([QueueTrigger(nameof(OrderProductGeneratorResult))] string queueMessage, DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt, int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount, TextWriter logger)
        //{
        //    var queueM = MessageQueue<OrderProductGeneratorResult>.GenerateQueueMessage(queueMessage, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount, queueTrigger, cloudStorageAccount);

        //    new ProductGeneratorResult().ProcessMessages(queueM);

        //    logger.WriteLine($"ProductGeneratorResult: {queueM.Data.OrderId} (f: {queueM.Data.FileUrl})");
        //}

        public static void BackupBDTrigger([TimerTrigger("0 0 10 * * 0", RunOnStartup = true)]TimerInfo myTimer, TextWriter log)
        {
            try
            {
                CloudStorageAccount storageAccount = AzureStorageAccount.DefaultAccount;
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(AzureStorageContainer.BackupsBD);
                blobContainer.CreateIfNotExists();

                string storageName = $"backup_{DateTime.Now.ToString("yyyyMMdd")}.bacpac";
                CloudBlockBlob backupFile = blobContainer.GetBlockBlobReference(storageName);

                string tempFile = $"{Path.GetTempPath()}{backupFile.Name}";

                DacServices services = new DacServices(@"Data Source=(LocalDB)\v11.0;User Id=mpetrini;Password=xxx");
                services.ExportBacpac(tempFile, "LIMBS_a534054da0bd4135a1c5c9674f2158f2");

                //backupFile.Properties.ContentType = "binary/octet-stream";
                backupFile.UploadFromFile(tempFile);

                log.WriteLine($"URL PRIMARIA BLOB: {backupFile.StorageUri.PrimaryUri}");
                log.WriteLine($"URL PRIMARIA BLOB: {backupFile.StorageUri.SecondaryUri}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
