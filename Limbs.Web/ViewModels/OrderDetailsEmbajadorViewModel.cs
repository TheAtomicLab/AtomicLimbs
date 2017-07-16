using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Limbs.Web.Models;

namespace Limbs.Web.ViewModels
{
    public class OrderDetailsEmbajadorViewModel
    {
        public OrderModel Order { get; set; }
        public bool CanChangeOrderStatus { get; set; }

    }
}