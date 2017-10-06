using System.Collections.Generic;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public interface IQueueMessageRemover<TMessage>
    {
        void RemoveProcessedMessages(IEnumerable<QueueMessage<TMessage>> sucefullyProcessedMessages);
        void RemoveProcessedMessage(QueueMessage<TMessage> sucefullyProcessedMessage);
    }
}
