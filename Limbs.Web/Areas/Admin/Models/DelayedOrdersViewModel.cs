using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Areas.Admin.Models
{
    public class DelayedOrdersViewModel 
    {

        [Display(Name = "Días", Description = "desde cambio de estado")]
        public int DaysBefore { get; set; }
        public IEnumerable<OrderModel> List { get; set; }
    }
}