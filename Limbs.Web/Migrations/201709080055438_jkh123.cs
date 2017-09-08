namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jkh123 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "AmbassadorLastName", c => c.String(nullable: false));
            AddColumn("dbo.AmbassadorModels", "Country", c => c.String(nullable: false));
            AddColumn("dbo.AmbassadorModels", "City", c => c.String(nullable: false));
            AddColumn("dbo.AmbassadorModels", "Phone", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "UserLastName", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "ResponsableLastName", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "Country", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "City", c => c.String(nullable: false));
            AlterColumn("dbo.AmbassadorModels", "Email", c => c.String());
            AlterColumn("dbo.UserModels", "Email", c => c.String());
            DropColumn("dbo.UserModels", "AmputationType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserModels", "AmputationType", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.AmbassadorModels", "Email", c => c.String(nullable: false));
            DropColumn("dbo.UserModels", "City");
            DropColumn("dbo.UserModels", "Country");
            DropColumn("dbo.UserModels", "ResponsableLastName");
            DropColumn("dbo.UserModels", "UserLastName");
            DropColumn("dbo.AmbassadorModels", "Phone");
            DropColumn("dbo.AmbassadorModels", "City");
            DropColumn("dbo.AmbassadorModels", "Country");
            DropColumn("dbo.AmbassadorModels", "AmbassadorLastName");
        }
    }
}
