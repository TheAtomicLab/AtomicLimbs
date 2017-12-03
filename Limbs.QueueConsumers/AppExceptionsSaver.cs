using System;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Limbs.Web.Storage.Azure.TableStorage;

namespace Limbs.QueueConsumers
{
    public class AppExceptionsSaver : IQueueMessageConsumer<AppException>
    {
        public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(40);
        public TimeSpan? EstimatedTimeToProcessMessageBlock { get; }

        private static TablePersister<AppExceptionData> _tablePersister;

        public AppExceptionsSaver()
        {
            EstimatedTimeToProcessMessageBlock = EstimatedTime;

            var tableClient = AzureStorageAccount.DefaultAccount.CreateCloudTableClient();
            _tablePersister = new TablePersister<AppExceptionData>(tableClient);
        }


        public void ProcessMessages(QueueMessage<AppException> message)
        {
            if (message.DequeueCount > 100)
                return;

            var messageLog = message.Data;
            _tablePersister.Add(new AppExceptionData(message.Id, messageLog.Date ?? (message.InsertionTime?.DateTime ?? DateTime.UtcNow))
            {
                CustomMessage = messageLog.CustomMessage,
                InnerExceptionMessage = messageLog.Exception.InnerException?.Message,
                Url = messageLog.Url,
                UrlReferrer = messageLog.UrlReferrer,
                Message = messageLog.Exception.Message,
                StackTrace = messageLog.Exception.StackTrace,
            });
        }
    }

}
