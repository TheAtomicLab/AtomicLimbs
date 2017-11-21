using System;
using System.Linq;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;

namespace Limbs.QueueConsumers
{
    public class ProductGeneratorResult : IQueueMessageConsumer<OrderProductGeneratorResult>
    {
        public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(5);
        
        public TimeSpan? EstimatedTimeToProcessMessageBlock { get; }

        public void ProcessMessages(QueueMessage<OrderProductGeneratorResult> message)
        {
            try
            {
                Console.WriteLine($"Result from ({message.Data.OrderId})");

                var db = ApplicationDbContext.Create();

                var order = db.OrderModels.FirstOrDefault(x => x.Id == message.Data.OrderId);

                if(order == null) throw new InvalidOperationException($"{nameof(message.Data.OrderId)}: {message.Data.OrderId}");

                order.LogMessage("OrderProductGeneratorResult: " + message.Data.FileUrl);
                order.FileUrl = message.Data.FileUrl;

                db.SaveChanges();
            }
            catch (Exception e)
            {
                e.Log(null, $"Actualizando orden ({message.Data.OrderId})");
                var exmsg = $"Actualizando orden ({message.Data.OrderId}): {e.Message}\nStackTrace:{e.StackTrace}";
                Console.WriteLine(exmsg, "Error");
                if (message.DequeueCount < 20)
                {
                    throw;
                }
            }
        }
    }
}
