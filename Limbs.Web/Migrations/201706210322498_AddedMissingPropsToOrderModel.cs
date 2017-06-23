namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddedPropsToOrderModel : DbMigration
    {
        public override void Up()
        {
           CreateTable(
                "dbo.AccessoryModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.String(),
                        ImageUrl = c.String(),
                        OrderModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderModels", t => t.OrderModel_Id)
                .Index(t => t.OrderModel_Id);
            
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        Accessory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccessoryModels", t => t.Accessory_Id)
                .Index(t => t.Accessory_Id);
            
            AddColumn("dbo.OrderModels", "Design", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccessoryModels", "OrderModel_Id", "dbo.OrderModels");
            DropForeignKey("dbo.Colors", "Accessory_Id", "dbo.AccessoryModels");
            DropIndex("dbo.Colors", new[] { "Accessory_Id" });
            DropIndex("dbo.AccessoryModels", new[] { "OrderModel_Id" });
            DropColumn("dbo.OrderModels", "Design");
            DropTable("dbo.Colors");
            DropTable("dbo.AccessoryModels");
        }
    }
}
