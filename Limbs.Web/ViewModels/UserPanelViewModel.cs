using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Limbs.Web.Models;

namespace Limbs.Web.ViewModels
{
    public class UserPanelViewModel
    {
        public List<OrderModel> Order { get; set; }

        public string Message { get; set; }
    }
}