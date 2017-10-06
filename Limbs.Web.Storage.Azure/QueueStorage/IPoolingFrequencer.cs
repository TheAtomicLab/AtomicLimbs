using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
    public interface IPoolingFrequencer
    {
        /// <summary>
        /// The current milliseconds.
        /// </summary>
        /// <remarks>> it increase at each call.</remarks>
        int Current { get; }

        void Decrease();
    }
}
