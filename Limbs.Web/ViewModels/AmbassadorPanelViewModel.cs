using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Limbs.Web.Models;

namespace Limbs.Web.ViewModels
{
    public class AmbassadorPanelViewModel
    {
        public OrderStats Stats { get; set; }
        public List<OrderModel> PendingToAssignOrders { get; set; }
        public List<OrderModel> PendingOrders { get; set; }
        public List<OrderModel> DeliveredOrders { get; set; }
        public bool PointIsValid { get; set; }
    }

    public class OrderStats
    {
        public int HandledOrders { get; set; }

        public int PendingOrders { get; set; }

    }
}