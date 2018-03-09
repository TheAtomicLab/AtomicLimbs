using System;
using System.Collections.Generic;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels
{
    public class UserPanelViewModel
    {
        public List<OrderModel> Order { get; set; }

        public string Message { get; set; }

        public DateTime UserBirth { get; set; }
    }
}