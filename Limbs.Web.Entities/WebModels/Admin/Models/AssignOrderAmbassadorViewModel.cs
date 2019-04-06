using System;
using System.Collections.Generic;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Entities.WebModels.Admin.Models
{
    public class AssignOrderAmbassadorViewModel
    {
        public AssignOrderAmbassadorViewModel()
        {
            AmbassadorList = new List<Tuple<AmbassadorModel, double,int>>();
        }

        public OrderModel Order { get; set; }
        public IEnumerable<Tuple<AmbassadorModel, double,int>> AmbassadorList { get; set; }
    }
}