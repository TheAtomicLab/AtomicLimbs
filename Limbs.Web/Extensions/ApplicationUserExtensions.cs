using System;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Extensions
{
    public static class ApplicationUserExtensions
    {

        public static ApplicationUserViewModel ToViewModel(this ApplicationUser applicationUser)
        {
            return new ApplicationUserViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
            };
        }
    }
}