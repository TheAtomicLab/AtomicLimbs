using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public abstract class MessagePoolQueueConsumer<TMessage> : AbstractQueueConsumer<TMessage> where TMessage : class
    {
        protected MessagePoolQueueConsumer(IQueueMessageConsumer<TMessage> consumer)
        {
            Consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        }

        protected IQueueMessageConsumer<TMessage> Consumer { get; }

        protected virtual string ConsumerName => Consumer.GetType().Name;

        public override void PollQueue()
        {
            var account = QueueAccount;
            var queue = new MessageQueue<TMessage>(account);
            var frequencer = Frequencer;
            var useDefaultTimeout = !Consumer.EstimatedTimeToProcessMessageBlock.HasValue;
            var defaultTimeout = useDefaultTimeout ? TimeSpan.FromSeconds(1) : Consumer.EstimatedTimeToProcessMessageBlock.Value;
            while (true)
            {
                try
                {
                    var message = useDefaultTimeout ? queue.Dequeue() : queue.Dequeue(defaultTimeout);
                    if (message != null)
                    {
                        try
                        {
                            Consumer.ProcessMessages(message);
                            queue.Remove(message);
                        }
                        catch (Exception e)
                        {
                            OnProcessMessageLogException(message, e);
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

        protected abstract CloudStorageAccount QueueAccount { get; }

        protected virtual void OnDequeueLogException(Exception e)
        {
            var message = $"Queue connection of {ConsumerName}: {e.Message}\nStackTrace:{e.StackTrace}";
            Trace.WriteLine(message, "Error");
        }

        protected virtual void OnProcessMessageLogException(QueueMessage<TMessage> message, Exception e)
        {
            var errorMessage = $"{ConsumerName}: {e.Message}\nStackTrace:{e.StackTrace}";
            Trace.WriteLine(errorMessage, "Error");
        }
    }
}
