using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System;

namespace Limbs.Web.Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            userIdentity.AddClaim(CheckIfAdmin(userIdentity.GetUserName())
                ? new Claim(ClaimTypes.Role, AppRoles.Administrator)
                : new Claim(ClaimTypes.Role, AppRoles.User));
            return userIdentity;
        }

        public static ApplicationUser AdminAlias()
        {
            PasswordHasher hasher = new PasswordHasher();
            return new ApplicationUser
            {
                Email = SuperAdminEmail,
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword("atomic01"),
                UserName = SuperAdminEmail,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            
        }

        public static string SuperAdminEmail = "limbs-admin@atomiclab.org";

        private static bool CheckIfAdmin(string email)
        {
            var admins = ConfigurationManager.AppSettings["AdminEmails"].Split(',').ToList();

            return admins.Contains(email);
        }
    }
}