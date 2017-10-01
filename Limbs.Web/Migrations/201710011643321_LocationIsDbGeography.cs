using System.Data.Entity.Migrations;
using System.Data.Entity.Spatial;

namespace Limbs.Web.Migrations
{
    public partial class LocationIsDbGeography : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "Location", c => c.Geography());
            Sql("UPDATE dbo.AmbassadorModels SET Location = geography::Point(-34.6037389,-58.3837591,4326)");
            AlterColumn("dbo.AmbassadorModels", "Location", c => c.Geography(false));

            AddColumn("dbo.UserModels", "Location", c => c.Geography());
            Sql("UPDATE dbo.UserModels SET Location = geography::Point(-34.6037389,-58.3837591,4326)");
            AlterColumn("dbo.UserModels", "Location", c => c.Geography(false));
            
            DropColumn("dbo.AmbassadorModels", "Lat");
            DropColumn("dbo.AmbassadorModels", "Long");
            DropColumn("dbo.UserModels", "Lat");
            DropColumn("dbo.UserModels", "Long");
        }

        public override void Down()
        {
            AddColumn("dbo.UserModels", "Long", c => c.Double(false));
            AddColumn("dbo.UserModels", "Lat", c => c.Double(false));
            AddColumn("dbo.AmbassadorModels", "Long", c => c.Double(false));
            AddColumn("dbo.AmbassadorModels", "Lat", c => c.Double(false));
            DropColumn("dbo.UserModels", "Location");
            DropColumn("dbo.AmbassadorModels", "Location");
        }
    }
}