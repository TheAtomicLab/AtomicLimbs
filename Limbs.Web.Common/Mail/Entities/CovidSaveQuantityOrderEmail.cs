using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbs.Web.Common.Mail.Entities
{
    public class CovidSaveQuantityOrderEmail
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string AmbassadorName { get; set; }
        public string AmbassadorLastname { get; set; }
        public string AmbassadorPhoneNumber { get; set; }
        public string AmbassadorEmail { get; set; }
        public string AmbassadorAddress { get; set; }
        public int Quantity { get; set; }
    }
}
