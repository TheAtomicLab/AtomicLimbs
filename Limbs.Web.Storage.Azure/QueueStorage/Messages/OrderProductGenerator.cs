using Limbs.Web.Entities.Models;

namespace Limbs.Web.Storage.Azure.QueueStorage.Messages
{
    public class OrderProductGenerator
    {
        public OrderSizesModel ProductSizes { get; set; }

        public int OrderId { get; set; }

        /// <summary>
        /// Pieces in false are not being generated, the ambassador has already printed 'true' ones
        /// </summary>
        public Pieces Pieces { get; set; }
    }
}
