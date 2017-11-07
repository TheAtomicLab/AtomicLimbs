using System;
using System.Security.Principal;
using Limbs.Web.Areas.Admin.Models;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;

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

        public static bool IsInMessage(this IPrincipal applicationUser, MessageModel messageModel)
        {
            var userId = applicationUser.Identity.GetUserId();
            return userId.Equals(messageModel.To.Id) || userId.Equals(messageModel.From.Id);
        }
        public static bool IsDestination(this IPrincipal applicationUser, MessageModel messageModel)
        {
            var userId = applicationUser.Identity.GetUserId();
            return userId.Equals(messageModel.To.Id);
        }
    }
}