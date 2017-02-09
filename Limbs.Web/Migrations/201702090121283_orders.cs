namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comments = c.String(),
                        Status = c.String(),
                        OrderRequestor_Id = c.Int(),
                        OrderUser_Id = c.Int(),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserModels", t => t.OrderRequestor_Id)
                .ForeignKey("dbo.UserModels", t => t.OrderUser_Id)
                .ForeignKey("dbo.ProductModels", t => t.Product_Id)
                .Index(t => t.OrderRequestor_Id)
                .Index(t => t.OrderUser_Id)
                .Index(t => t.Product_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderModels", "Product_Id", "dbo.ProductModels");
            DropForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.UserModels");
            DropForeignKey("dbo.OrderModels", "OrderRequestor_Id", "dbo.UserModels");
            DropIndex("dbo.OrderModels", new[] { "Product_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderUser_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropTable("dbo.OrderModels");
        }
    }
}
