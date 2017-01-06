namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somechanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserModels", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Phone", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Gender", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Country", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "City", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Address", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserModels", "Address", c => c.String());
            AlterColumn("dbo.UserModels", "City", c => c.String());
            AlterColumn("dbo.UserModels", "Country", c => c.String());
            AlterColumn("dbo.UserModels", "Gender", c => c.String());
            AlterColumn("dbo.UserModels", "Phone", c => c.String());
            AlterColumn("dbo.UserModels", "Email", c => c.String());
            AlterColumn("dbo.UserModels", "LastName", c => c.String());
            AlterColumn("dbo.UserModels", "Name", c => c.String());
        }
    }
}
