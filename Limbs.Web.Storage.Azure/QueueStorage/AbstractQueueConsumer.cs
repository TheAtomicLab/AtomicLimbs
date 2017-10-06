using System;
using System.Threading;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public abstract class AbstractQueueConsumer<TMessage> : IQueueConsumer
    {
        private IPoolingFrequencer _frequencer;
        private Thread _pollingThread;

        public IQueueConsumer With(IPoolingFrequencer theFrequencer)
        {
            _frequencer = theFrequencer ?? throw new ArgumentNullException(nameof(theFrequencer));
            return this;
        }

        public virtual IPoolingFrequencer Frequencer
        {
            get
            {
                if (_frequencer == null)
                {
                    // The Azure default for message-timeout visibility is 30"
                    const int defaultFloor = 60 * 1000; // one minute
                    const int defaultCeiling = 10 * 60 * 1000; // 10 minutes
                    _frequencer = new PoolingFrequencer(defaultFloor, defaultCeiling);
                }
                return _frequencer;
            }
        }

        public void StartConsimung()
        {
            _pollingThread = CreateThreadForPolling();
            if (_pollingThread != null)
            {
                _pollingThread.Start();
            }
            else
            {
                PollQueue();
            }
        }

        public void StopConsimung()
        {
            _pollingThread?.Abort();
        }

        protected abstract Thread CreateThreadForPolling();
        public abstract void PollQueue();
    }
}
