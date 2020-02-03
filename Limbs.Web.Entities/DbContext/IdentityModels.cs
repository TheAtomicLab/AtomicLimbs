using System.Data.Entity;
using Limbs.Web.Entities.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Limbs.Web.Entities.DbContext
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    // TODO (ale): refactor dbContext as Interface
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("name=Limbs", false) { }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<EventModel> EventModels { get; set; }
        public DbSet<EventTypeModel> EventTypeModels { get; set; }
        public DbSet<EventOrderModel> EventOrderModels { get; set; }

        public DbSet<RenderModel> RenderModels { get; set; }
        public DbSet<AmputationTypeModel> AmputationTypeModels { get; set; }
        public DbSet<ColorModel> ColorModels { get; set; }
        public DbSet<OrderRenderPieceModel> OrderRenderPieceModels { get; set; }
        public DbSet<RenderPieceModel> RenderPieceModels { get; set; }

        public DbSet<OrderRefusedModels> OrderRefusedModels { get; set; }
        public DbSet<UserModel> UserModelsT { get; set; }

        public DbSet<AmbassadorModel> AmbassadorModels { get; set; }

        public DbSet<OrderModel> OrderModels { get; set; }

        public DbSet<MessageModel> Messages { get; set; }

        public DbSet<SponsorModel> SponsorModels { get; set; }
    }
}