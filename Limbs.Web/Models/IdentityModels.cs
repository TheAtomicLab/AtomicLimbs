using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Limbs.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
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

        private static bool CheckIfAdmin(string email)
        {
            var admins = ConfigurationManager.AppSettings["AdminEmails"].Split(';').ToList();

            return admins.Contains(email);
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("name=Limbs", false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<UserModel> UserModelsT { get; set; }

        public DbSet<AmbassadorModel> AmbassadorModels { get; set; }
        
        public DbSet<OrderModel> OrderModels { get; set; }
    }
}