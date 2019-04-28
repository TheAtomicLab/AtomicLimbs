namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class refusedOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderRefusedModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        AmbassadorId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmbassadorModels", t => t.AmbassadorId, cascadeDelete: true)
                .ForeignKey("dbo.OrderModels", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.AmbassadorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRefusedModels", "OrderId", "dbo.OrderModels");
            DropForeignKey("dbo.OrderRefusedModels", "AmbassadorId", "dbo.AmbassadorModels");
            DropIndex("dbo.OrderRefusedModels", new[] { "AmbassadorId" });
            DropIndex("dbo.OrderRefusedModels", new[] { "OrderId" });
            DropTable("dbo.OrderRefusedModels");
        }
    }
}
