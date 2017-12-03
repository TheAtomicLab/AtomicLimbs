using System;
using System.IO;
using Limbs.QueueConsumers;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Limbs.Worker
{
    public class Functions
    {
        public static void ProcessAppExceptions([QueueTrigger(nameof(AppException))] string queueMessage, TextWriter logger)
        {
            var message = JsonConvert.DeserializeObject<AppException>(queueMessage);

            logger.WriteLine(message);
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
    }
}
