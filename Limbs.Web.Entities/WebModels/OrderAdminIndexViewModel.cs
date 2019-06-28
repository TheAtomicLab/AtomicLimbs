using Limbs.Web.Entities.Models;
using System;

namespace Limbs.Web.Entities.WebModels
{
    public class OrderAdminIndexViewModel
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public string FileUrl { get; set; }
        public int PercentagePrinted { get; set; }
        public Courier Courier { get; set; }
        public string ProofOfDelivery { get; set; }
        public string AmputationDescription { get; set; }
        public DateTime Date { get; set; }
        public string RequesterEmail { get; set; }
        public string AmbassadorEmail { get; set; }
        public bool HasDesign { get; set; }
    }
}