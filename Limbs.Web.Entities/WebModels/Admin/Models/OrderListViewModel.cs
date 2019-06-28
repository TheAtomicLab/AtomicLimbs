using System.Collections.Generic;

namespace Limbs.Web.Entities.WebModels.Admin.Models
{
    public class OrderListViewModel
    {
        public IEnumerable<OrderAdminIndexViewModel> List { get; set; }
        public OrderFilters Filters { get; set; }
    }
}