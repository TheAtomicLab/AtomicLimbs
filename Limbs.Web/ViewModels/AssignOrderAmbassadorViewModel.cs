using System.Collections.Generic;
using Limbs.Web.Models;

namespace Limbs.Web.ViewModels
{
    public class AssignOrderAmbassadorViewModel
    {
        public AssignOrderAmbassadorViewModel()
        {
            AmbassadorList = new List<AmbassadorModel>();
        }

        public OrderModel Order { get; set; }
        public IEnumerable<AmbassadorModel> AmbassadorList { get; set; }
    }
}