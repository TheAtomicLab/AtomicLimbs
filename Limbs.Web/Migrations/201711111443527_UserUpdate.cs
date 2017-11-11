namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserModels", "IsProductUser", c => c.Boolean(nullable: false));
            AlterColumn("dbo.UserModels", "UserId", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "ResponsableName", c => c.String());
            AlterColumn("dbo.UserModels", "ResponsableLastName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserModels", "ResponsableLastName", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "ResponsableName", c => c.String(nullable: false));
            AlterColumn("dbo.UserModels", "Email", c => c.String());
            AlterColumn("dbo.UserModels", "UserId", c => c.String());
            DropColumn("dbo.UserModels", "IsProductUser");
        }
    }
}
