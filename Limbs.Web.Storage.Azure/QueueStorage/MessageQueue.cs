using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public static class AzureQueue
    {
        public static void Enqueue<TM>(TM message) where TM : class
        {
            var mq = new MessageQueue<TM>();
            mq.Enqueue(message);
        }

        public static Task EnqueueAsync<TM>(TM message) where TM : class
        {
            return Task.Run(() => Enqueue(message));
        }
    }

    /// <summary>
    /// Generic base class for messages.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <remarks>
    /// The <typeparamref name="TMessage"/> have to be JSON serializable.
    /// </remarks>
    public class MessageQueue<TMessage> where TMessage : class
    {
        private const int MaxMessageBlockAllowedByAzure = 32;
        private readonly CloudQueueClient _queueClient;
        private readonly string _queueName = typeof(TMessage).Name.ToLowerInvariant();

        public MessageQueue() : this(AzureStorageAccount.DefaultAccount)
        {
        }

        public MessageQueue(CloudStorageAccount account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            _queueClient = account.CreateCloudQueueClient();
        }

        public int ApproximateMessageCount
        {
            get
            {
                var queueRef = _queueClient.GetQueueReference(_queueName);
                var count = queueRef.ApproximateMessageCount;
                return count ?? 0;
            }
        }

        public void Enqueue(TMessage messageContent)
        {
            if (messageContent == null)
            {
                throw new ArgumentNullException(nameof(messageContent));
            }
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var message = new CloudQueueMessage(SerializeObjectAsString(messageContent));
            queueRef.AddMessage(message);
        }

        public QueueMessage<TMessage> Dequeue()
        {
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var message = queueRef.GetMessage();
            return ConvertToQueueMessage(message);
        }

        public IEnumerable<QueueMessage<TMessage>> Dequeue(int messagesCount)
        {
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var messages = queueRef.GetMessages(messagesCount < MaxMessageBlockAllowedByAzure
                ? messagesCount
                : MaxMessageBlockAllowedByAzure);
            return messages.Select(ConvertToQueueMessage);
        }

        public QueueMessage<TMessage> Dequeue(TimeSpan timeout)
        {
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var message = queueRef.GetMessage(timeout);
            return ConvertToQueueMessage(message);
        }

        public IEnumerable<QueueMessage<TMessage>> Dequeue(int messagesCount, TimeSpan timeout)
        {
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var messages =
                queueRef.GetMessages(
                    messagesCount < MaxMessageBlockAllowedByAzure ? messagesCount : MaxMessageBlockAllowedByAzure,
                    timeout);
            return messages.Select(ConvertToQueueMessage);
        }

        public QueueMessage<TMessage> Peek()
        {
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var message = queueRef.PeekMessage();
            return ConvertToQueueMessage(message);
        }

        public IEnumerable<QueueMessage<TMessage>> Peek(int messagesCount)
        {
            var queueRef = _queueClient.GetQueueReference(_queueName);
            var messages = queueRef.PeekMessages(messagesCount < MaxMessageBlockAllowedByAzure
                ? messagesCount
                : MaxMessageBlockAllowedByAzure);
            return messages.Select(ConvertToQueueMessage);
        }

        public void Remove(QueueMessage<TMessage> queueMessage)
        {
            if (queueMessage == null)
            {
                throw new ArgumentNullException(nameof(queueMessage));
            }
            var queueRef = _queueClient.GetQueueReference(_queueName);
            queueRef.DeleteMessage(queueMessage.Id, queueMessage.PopReceipt);
        }

        public void Remove(IEnumerable<QueueMessage<TMessage>> queueMessages)
        {
            if (queueMessages == null)
            {
                throw new ArgumentNullException(nameof(queueMessages));
            }
            var queueRef = _queueClient.GetQueueReference(_queueName);
            foreach (var queueMessage in queueMessages)
            {
                queueRef.DeleteMessage(queueMessage.Id, queueMessage.PopReceipt);
            }
        }

        protected virtual string SerializeObjectAsString(TMessage messageContent)
        {
            // a subclass can gzipr the message (GZipStream) where the serialized TMessage is > 8KB
            return JsonConvert.SerializeObject(messageContent);
        }

        protected static TMessage DeserializeObjectFromString(string messageContent)
        {
            // a subclass can de-gzip the message
            return JsonConvert.DeserializeObject<TMessage>(messageContent);
        }

        protected static QueueMessage<TMessage> ConvertToQueueMessage(CloudQueueMessage message)
        {
            if (message == null)
            {
                return null;
            }
            string messageContent = message.AsString;
            return new QueueMessage<TMessage>
            {
                Id = message.Id,
                PopReceipt = message.PopReceipt,
                DequeueCount = message.DequeueCount,
                InsertionTime = message.InsertionTime,
                ExpirationTime = message.ExpirationTime,
                NextVisibleTime = message.NextVisibleTime,
                Data = DeserializeObjectFromString(messageContent)
            };
        }

        public static QueueMessage<TMessage> GenerateQueueMessage(string messageContent, DateTimeOffset expirationTime,
            DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime, string id, string popReceipt,
            int dequeueCount, string queueTrigger, CloudStorageAccount cloudStorageAccount)
        {
            return new QueueMessage<TMessage>
            {
                Id = id,
                PopReceipt = popReceipt,
                DequeueCount = dequeueCount,
                InsertionTime = insertionTime,
                ExpirationTime = expirationTime,
                NextVisibleTime = nextVisibleTime,
                Data = DeserializeObjectFromString(messageContent),
            };
        }
    }
}