namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeUserEvent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EventUserModels", "EventId", "dbo.EventModels");
            DropForeignKey("dbo.EventUserModels", "UserId", "dbo.UserModels");
            DropIndex("dbo.EventUserModels", new[] { "UserId" });
            DropIndex("dbo.EventUserModels", new[] { "EventId" });
            CreateTable(
                "dbo.EventOrderModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        Participated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventModels", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.OrderModels", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.EventId);
            
            DropTable("dbo.EventUserModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EventUserModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                        Participated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.EventOrderModels", "OrderId", "dbo.OrderModels");
            DropForeignKey("dbo.EventOrderModels", "EventId", "dbo.EventModels");
            DropIndex("dbo.EventOrderModels", new[] { "EventId" });
            DropIndex("dbo.EventOrderModels", new[] { "OrderId" });
            DropTable("dbo.EventOrderModels");
            CreateIndex("dbo.EventUserModels", "EventId");
            CreateIndex("dbo.EventUserModels", "UserId");
            AddForeignKey("dbo.EventUserModels", "UserId", "dbo.UserModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EventUserModels", "EventId", "dbo.EventModels", "Id", cascadeDelete: true);
        }
    }
}
