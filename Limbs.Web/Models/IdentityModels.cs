using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
                ? new Claim(ClaimTypes.Role, "Administrator")
                : new Claim(ClaimTypes.Role, "User"));
            return userIdentity;
        }

        private bool CheckIfAdmin(string email)
        {
            var admins = ConfigurationManager.AppSettings["AdminEmails"].Split(';').ToList();

            return admins.Contains(email);
        }
    }
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        
        public System.Data.Entity.DbSet<Limbs.Web.Models.AccessoryModel> AccessoryModels { get; set; }

        public System.Data.Entity.DbSet<Limbs.Web.Models.Color> Colors { get; set; }

        public System.Data.Entity.DbSet<Limbs.Web.Models.UserModel> UserModels { get; set; }

        public System.Data.Entity.DbSet<Limbs.Web.Models.FileModel> FileModels { get; set; }

        public System.Data.Entity.DbSet<Limbs.Web.Models.ProductModel> ProductModels { get; set; }
    }
}