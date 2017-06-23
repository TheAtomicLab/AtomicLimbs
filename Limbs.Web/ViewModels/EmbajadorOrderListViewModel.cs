using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Limbs.Web.Models;

namespace Limbs.Web.ViewModels
{
    public class EmbajadorOrderListViewModel
    {
        public IEnumerable<OrderModel> PendingToAssignOrders { get; set; }
        public IEnumerable<OrderModel> AssignedOrders { get; set; }
    }
}