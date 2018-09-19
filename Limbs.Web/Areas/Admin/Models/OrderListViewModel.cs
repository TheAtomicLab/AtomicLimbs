using System.Collections.Generic;
using Limbs.Web.Entities.Models;
using Limbs.Web.Services;

namespace Limbs.Web.Areas.Admin.Models
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderModel> List { get; set; }
        public OrderFilters Filters { get; set; }
    }
}