using System;
using System.Collections.Generic;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels
{
    public class COVIDInfoAdminViewModel
    {
        public int TotalQuantity { get; set; }
        public int TotalOrders { get; set; }
        public List<CovidOrganizationModel> RankingOrders { get; set; }
        public int TotalAssignedAmbassadors { get; set; }
        public int TotalAssignedQuantity { get; set; }        
        public List<CovidOrgAmbassador> AssignmentList { get; set; }
    }
}