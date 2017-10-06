using System;
using System.Collections.Generic;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Areas.Admin.Models
{
    public class AssignOrderAmbassadorViewModel
    {
        public AssignOrderAmbassadorViewModel()
        {
            AmbassadorList = new List<Tuple<AmbassadorModel, double>>();
        }

        public OrderModel Order { get; set; }
        public IEnumerable<Tuple<AmbassadorModel, double>> AmbassadorList { get; set; }
    }
}