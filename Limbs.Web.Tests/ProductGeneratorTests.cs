using System;
using System.Threading;
using Limbs.QueueConsumers;
using Limbs.Web.Entities.Models;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Limbs.Web.Tests
{
    [TestClass]
    public class ProductGeneratorTests
    {
        [TestMethod]
        public void StartConsumigProductGeneratorResult()
        {
            QueueConsumerFor<OrderProductGeneratorResult>.WithinCurrentThread.Using(new ProductGeneratorResult())
                .With(PoolingFrequencer.For(ProductGeneratorResult.EstimatedTime))
                .StartConsimung();
        }

        [TestMethod]
        public void StartConsumigProductGeneratorResultManual()
        {
            var storageAccount = AzureStorageAccount.DefaultAccount;

            // Create the queue client
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            var queue = queueClient.GetQueueReference("orderproductgeneratorresult");

            while (true)
            {
                // Peek at the next message
                var peekedMessage = queue.PeekMessage();

                // Display message.
                Console.WriteLine(peekedMessage.AsString);

                Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void EnqueueProductGenerator()
        {
            for (int i = 0; i < 5; i++)
            {
                AzureQueue.Enqueue(new OrderProductGenerator
                {
                    OrderId = new Random().Next(1, 100),
                    Pieces = new Pieces(true),
                    ProductSizes = new OrderSizesModel
                    {
                        A = 0.2f,
                        B = 0.3f,
                        C = 0.4f,
                    },
                });
            }
        }
    }
}
