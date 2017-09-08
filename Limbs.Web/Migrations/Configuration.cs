namespace Limbs.Web.Migrations
{
    using Limbs.Web.Models;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Unassigned"));
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Requester"));
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Ambassador"));
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin"));
            
            context.SaveChanges();

        }
    }
}
