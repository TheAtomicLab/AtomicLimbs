using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Limbs.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string[] Roles { get; set; }

        public string[] LoginProviders { get; set; }
    }
}