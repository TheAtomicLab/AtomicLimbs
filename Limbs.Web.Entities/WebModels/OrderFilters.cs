using Limbs.Web.Entities.Models;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Por estado:", Description = "")]
        public bool ByStatus { get; set; }
        public OrderStatus Status { get; set; }

        //esto se podria hacer con el type nullable y sin bool, pero romperia por todos lados
        [Display(Name = "Por tipo:", Description = "")]
        public bool ByAmputationType { get; set; }
        public AmputationType AmputationType { get; set; }

        [Display(Name = "Busqueda:", Description = "")]
        public string SearchTerm { get; set; }
    }
}
