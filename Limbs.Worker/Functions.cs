using System;
using System.IO;
using Limbs.QueueConsumers;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.Azure.WebJobs;

namespace Limbs.Worker
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static void MailsMessagesSender(TextWriter log)
        {
            try
            {
                Console.WriteLine("MailsMessagesSender");

                QueueConsumerFor<MailMessage>.WithStandaloneThread.Using(new MailsMessagesSender())
                    .With(PoolingFrequencer.For(QueueConsumers.MailsMessagesSender.EstimatedTime))
                    .StartConsimung();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MailsMessagesSender error");
                Console.WriteLine(ex);
                log.WriteLine("Error occurred in processing pending requests. Error : {0}", ex.Message);
            }
        }


        [NoAutomaticTrigger]
        public static void ProductGeneratorResult(TextWriter log)
        {
            try
            {
                Console.WriteLine("ProductGeneratorResult");

                QueueConsumerFor<OrderProductGeneratorResult>.WithStandaloneThread.Using(new ProductGeneratorResult())
                    .With(PoolingFrequencer.For(QueueConsumers.ProductGeneratorResult.EstimatedTime))
                    .StartConsimung();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProductGeneratorResult error");
                Console.WriteLine(ex);
                log.WriteLine("Error occurred in processing pending requests. Error : {0}", ex.Message);
            }
        }
    }
}
