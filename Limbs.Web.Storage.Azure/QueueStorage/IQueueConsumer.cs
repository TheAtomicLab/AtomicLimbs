using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public interface IQueueConsumer
    {
        IQueueConsumer With(IPoolingFrequencer frequencer);
        void StartConsimung();
        void StopConsimung();
    }
}
