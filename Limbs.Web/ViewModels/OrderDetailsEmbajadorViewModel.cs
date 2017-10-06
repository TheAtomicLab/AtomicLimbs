using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels
{
    public class OrderDetailsEmbajadorViewModel
    {
        public OrderModel Order { get; set; }
        public bool CanChangeOrderStatus { get; set; }

    }
}