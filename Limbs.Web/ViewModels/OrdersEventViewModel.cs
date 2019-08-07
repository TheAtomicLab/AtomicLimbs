using System;

namespace Limbs.Web.ViewModels
{
    public class OrdersEventViewModel
    {
        public int OrderId { get; set; }
        public string EventName { get; set; }
        public string OrderDate { get; set; }
        public string AmputationDescription { get; set; }
        public string RequesterEmail { get; set; }
        public string AmbassadorEmail { get; set; }
    }
}