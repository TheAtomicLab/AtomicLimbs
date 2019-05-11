using Limbs.QueueConsumers;
using Limbs.Web.Common.Mail;
using Limbs.Web.Common.Mail.Entities;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;
using Limbs.Web.Logic.Services;
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
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Limbs.Worker
{
    public class Functions
    {
        private ApplicationDbContext Db = new ApplicationDbContext();
        private readonly OrderService _os;
        private readonly IOrderNotificationService _ns;
        private readonly string _fromEmail = ConfigurationManager.AppSettings["Mail.From"];
        public Functions(IOrderNotificationService notificationService)
        {
            _ns = notificationService;
            _os = new OrderService(Db);
        }

        public async Task FollowUpAmbassadors([TimerTrigger("* 0 10 * * *", RunOnStartup = true)]TimerInfo myTimer, TextWriter log)
        {
            var ordersNoChange = await Db.OrderModels.Where(p => p.Status != OrderStatus.Delivered && p.Status != OrderStatus.NotAssigned && p.Status != OrderStatus.Rejected &&
                                    DbFunctions.AddDays(p.StatusLastUpdated, 7) <= DateTime.UtcNow).ToListAsync();

            if (ordersNoChange != null && ordersNoChange.Any())
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = _fromEmail,
                    Subject = "[Atomic Limbs] ¿Cómo va la impresión de la prótesis?"
                };

                foreach (var orderInList in ordersNoChange)
                {
                    var order = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).FirstOrDefaultAsync( p => p.Id == orderInList.Id);
                    if (order == null) continue;

                    var modelFollowUp = new FollowUpModel
                    {
                        AmbassadorName = order.OrderAmbassador.FullName(),
                        UserOrderName = order.OrderRequestor.FullName()
                    };

                    mailMessage.To = order.OrderAmbassador.Email;
                    mailMessage.Body = CompiledTemplateEngine.Render("Mails.FollowUpAmbassador", modelFollowUp);

                    await AzureQueue.EnqueueAsync(mailMessage);

                    order.StatusLastUpdated = DateTime.UtcNow;
                    order.LogMessage("Notification had sent (email follow-up) for inactivity in order");

                    Db.OrderModels.AddOrUpdate(order);
                }

                await Db.SaveChangesAsync();
            }
        }

        public async Task AssignAutomaticAmbassadorAfterThreeDaysAsync([TimerTrigger("* 0/5 * * * *", RunOnStartup = true)]TimerInfo myTimer, TextWriter log)
        {
            var ordersNoUpdate = await Db.OrderModels.Include(x => x.OrderAmbassador).Include(x => x.OrderRequestor).Where(p => p.Status == OrderStatus.Rejected &&
                                    DbFunctions.AddDays(p.StatusLastUpdated, 3) <= DateTime.UtcNow).ToListAsync();

            if (ordersNoUpdate != null && ordersNoUpdate.Any())
            {
                foreach (var order in ordersNoUpdate)
                {
                    var ambassador = await _os.GetAmbassadorAutoAssignAsync(order);
                    if (ambassador == null) continue;

                    await _os.AssignmentAmbassadorAsync(ambassador.Id, order.Id, null, _ns);
                }
            }
        }

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
