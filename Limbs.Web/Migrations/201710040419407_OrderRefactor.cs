namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderRefactor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderModels", "Sizes_Id", "dbo.OrderSizesModels");
            DropIndex("dbo.OrderModels", new[] { "Sizes_Id" });
            AddColumn("dbo.OrderModels", "ProductType", c => c.Int(nullable: false));
            AddColumn("dbo.OrderModels", "SizesData", c => c.String());
            AddColumn("dbo.OrderModels", "AmputationType", c => c.Int(nullable: false));
            DropColumn("dbo.OrderModels", "Sizes_Id");
            DropColumn("dbo.UserModels", "ProthesisType");
            DropColumn("dbo.UserModels", "ProductType");
            DropColumn("dbo.UserModels", "AmputationType");
            DropTable("dbo.OrderSizesModels");
            DropTable("dbo.ProductModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Thumbnail = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderSizesModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        A = c.Single(nullable: false),
                        B = c.Single(nullable: false),
                        C = c.Single(nullable: false),
                        D = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserModels", "AmputationType", c => c.Int(nullable: false));
            AddColumn("dbo.UserModels", "ProductType", c => c.Int(nullable: false));
            AddColumn("dbo.UserModels", "ProthesisType", c => c.Int(nullable: false));
            AddColumn("dbo.OrderModels", "Sizes_Id", c => c.Int());
            DropColumn("dbo.OrderModels", "AmputationType");
            DropColumn("dbo.OrderModels", "SizesData");
            DropColumn("dbo.OrderModels", "ProductType");
            CreateIndex("dbo.OrderModels", "Sizes_Id");
            AddForeignKey("dbo.OrderModels", "Sizes_Id", "dbo.OrderSizesModels", "Id");
        }
    }
}
