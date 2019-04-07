using System.Collections.Generic;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Entities.WebModels.Admin.Models
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderModel> List { get; set; }
        public OrderFilters Filters { get; set; }
    }
}