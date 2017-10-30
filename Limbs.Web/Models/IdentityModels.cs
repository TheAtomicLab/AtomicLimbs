using System.Data.Entity;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Limbs.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

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

        public DbSet<MessageModel> Messages { get; set; }
    }
}