using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public abstract class MessageBlockPoolQueueConsumer<TMessage> : AbstractQueueConsumer<TMessage> where TMessage : class
    {
        protected MessageBlockPoolQueueConsumer(IQueueMessageBlocksConsumer<TMessage> consumer)
        {
            Consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        }

        protected IQueueMessageBlocksConsumer<TMessage> Consumer { get; private set; }

        protected virtual string ConsumerName => Consumer.GetType().Name;

        protected abstract CloudStorageAccount QueueAccount { get; }

        public override void PollQueue()
        {
            CloudStorageAccount account = QueueAccount;
            var queue = new MessageQueue<TMessage>(account);
            var queueRemover = GetQueueRemover(queue);
            IPoolingFrequencer frequencer = Frequencer;
            bool useDefaultTimeout = !Consumer.EstimatedTimeToProcessMessageBlock.HasValue;
            TimeSpan defaultTimeout = useDefaultTimeout ? TimeSpan.FromSeconds(30) : Consumer.EstimatedTimeToProcessMessageBlock.Value;
            while (true)
            {
                try
                {
                    var messages = useDefaultTimeout ? queue.Dequeue(Consumer.BlockSize).ToList() : queue.Dequeue(Consumer.BlockSize, defaultTimeout).ToList();
                    if (messages.Count > 0)
                    {
                        try
                        {
                            Consumer.ProcessMessagesGroup(queueRemover, messages);
                        }
                        catch (Exception e)
                        {
                            OnProcessMessageLogException(messages, e);
                        }
                        finally
                        {
                            frequencer.Decrease();
                        }
                    }
                    else
                    {
                        Thread.Sleep(frequencer.Current);
                    }
                }
                catch (Exception e)
                {
                    OnDequeueLogException(e);
                    Thread.Sleep(5 * 1000);
                }
            }
        }

        protected virtual IQueueMessageRemover<TMessage> GetQueueRemover(MessageQueue<TMessage> queue)
        {
            return new QueueMessageRemover<TMessage>(queue);
        }

        protected virtual void OnDequeueLogException(Exception e)
        {
            string message = $"Queue connection of {ConsumerName}: {e.Message}\nStackTrace:{e.StackTrace}";
            Trace.WriteLine(message, "Error");
        }

        protected virtual void OnProcessMessageLogException(IEnumerable<QueueMessage<TMessage>> messages, Exception e)
        {
            string errorMessage = $"{ConsumerName}: {e.Message}\nStackTrace:{e.StackTrace}";
            Trace.WriteLine(errorMessage, "Error");
        }

        protected class QueueMessageRemover<TMessage> : IQueueMessageRemover<TMessage> where TMessage : class
        {
            private readonly MessageQueue<TMessage> _queue;

            public QueueMessageRemover(MessageQueue<TMessage> queue)
            {
                _queue = queue;
            }

            public void RemoveProcessedMessages(IEnumerable<QueueMessage<TMessage>> sucefullyProcessedMessages)
            {
                _queue.Remove(sucefullyProcessedMessages);
            }

            public void RemoveProcessedMessage(QueueMessage<TMessage> sucefullyProcessedMessage)
            {
                _queue.Remove(sucefullyProcessedMessage);
            }
        }
    }
}
