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
            if (context.Roles.Any()) return;

            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Unassigned));
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Requester));
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Ambassador));
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Administrator));

            context.SaveChanges();
        }
    }
}
