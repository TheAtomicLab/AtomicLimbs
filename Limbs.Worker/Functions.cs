using System;
using System.IO;
using System.Threading.Tasks;
using Limbs.QueueConsumers;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.Azure.WebJobs;

namespace Limbs.Worker
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        [NoAutomaticTrigger]
        public static async Task ProcessMethod(TextWriter log)
        {
            try
            {
                Console.WriteLine("ProcessMethod");

                QueueConsumerFor<MailMessage>.WithinCurrentThread.Using(new MailsMessagesSender())
                    .With(PoolingFrequencer.For(MailsMessagesSender.EstimatedTime))
                    .StartConsimung();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProcessMethod error");
                Console.WriteLine(ex);
                log.WriteLine("Error occurred in processing pending altapay requests. Error : {0}", ex.Message);
            }

            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(3));
            }
        }
    }
}
