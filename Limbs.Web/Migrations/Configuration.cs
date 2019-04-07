using System;
using System.Data.Entity.Validation;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;

    public sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            try
            {
                ApplicationUser admin = ApplicationUser.AdminAlias();
                if (!context.Users.Any(p => p.UserName == admin.UserName))
                    context.Users.AddOrUpdate(ApplicationUser.AdminAlias());

                if (!context.Roles.Any())
                {
                    context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Unassigned));
                    context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Requester));
                    context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Ambassador));
                    context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(AppRoles.Administrator));
                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Console.WriteLine(e);
                Console.WriteLine(nameof(e.EntityValidationErrors));
                StringBuilder sb = new StringBuilder();
                foreach (var dbEntityValidationResult in e.EntityValidationErrors)
                {
                    foreach (var validateError in dbEntityValidationResult.ValidationErrors)
                    {
                        sb.AppendLine($@"{validateError.PropertyName}: {validateError.ErrorMessage}");
                        Console.WriteLine($@"{validateError.PropertyName}: {validateError.ErrorMessage}");
                    }
                }
                throw new Exception(sb.ToString());
            }
        }
    }
}
