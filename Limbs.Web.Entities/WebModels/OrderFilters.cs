using Limbs.Web.Entities.Models;
using System.ComponentModel.DataAnnotations;
using Limbs.Web.Entities.Resources;

namespace Limbs.Web.Entities.WebModels
{
    public class OrderFilters : EntityPageFilter
    {
        public OrderFilters()
        {
            ByAmputationType = false;
            ByStatus = false;
        }

        //esto se podria hacer con el type nullable y sin bool, pero romperia por todos lados
        [Display(Name = "OrderFilters_Status", Description = "", ResourceType = typeof(ModelTexts))]
        public bool ByStatus { get; set; }
        public OrderStatus Status { get; set; }

        //esto se podria hacer con el type nullable y sin bool, pero romperia por todos lados
        [Display(Name = "OrderFilters_AmputationType", Description = "", ResourceType = typeof(ModelTexts))]
        public bool ByAmputationType { get; set; }
        public AmputationType AmputationType { get; set; }

        [Display(Name = "OrderFilters_SearchTerm", Description = "", ResourceType = typeof(ModelTexts))]
        public string SearchTerm { get; set; }
    }
}
