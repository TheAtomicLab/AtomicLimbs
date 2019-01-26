namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationWithUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AmbassadorModels", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.UserModels", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AmbassadorModels", "UserId");
            CreateIndex("dbo.UserModels", "UserId");
            AddForeignKey("dbo.UserModels", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AmbassadorModels", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AmbassadorModels", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserModels", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserModels", new[] { "UserId" });
            DropIndex("dbo.AmbassadorModels", new[] { "UserId" });
            AlterColumn("dbo.UserModels", "UserId", c => c.String(nullable: false));
            AlterColumn("dbo.AmbassadorModels", "UserId", c => c.String());
        }
    }
}
