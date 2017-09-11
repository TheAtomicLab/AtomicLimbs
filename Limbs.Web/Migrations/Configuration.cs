namespace Limbs.Web.Migrations
{
    using Limbs.Web.Models;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {

            if (context.Roles.Count() == 0)
            {
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Unassigned"));
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Requester"));
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Ambassador"));
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin"));

                context.SaveChanges();
            }

        }
    }
}
