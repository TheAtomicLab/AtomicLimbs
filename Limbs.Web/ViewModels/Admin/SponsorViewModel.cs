using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Limbs.Web.ViewModels.Admin
{
    public class SponsorViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventDescription { get; set; }
        public string WebImage { get; set; }
        public string MobileImage { get; set; }
    }
}