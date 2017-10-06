using System;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public interface IQueueMessageConsumer<TMessage>
    {
        TimeSpan? EstimatedTimeToProcessMessageBlock { get; }
        void ProcessMessages(QueueMessage<TMessage> message);
    }
}
