namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEventsModels : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventModels", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.UserModels", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.EventModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventTypeId = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        EventyType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventTypeModels", t => t.EventyType_Id)
                .Index(t => t.EventyType_Id);
            
            CreateTable(
                "dbo.EventTypeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventUserModels", "UserId", "dbo.UserModels");
            DropForeignKey("dbo.EventModels", "EventyType_Id", "dbo.EventTypeModels");
            DropForeignKey("dbo.EventUserModels", "EventId", "dbo.EventModels");
            DropIndex("dbo.EventModels", new[] { "EventyType_Id" });
            DropIndex("dbo.EventUserModels", new[] { "EventId" });
            DropIndex("dbo.EventUserModels", new[] { "UserId" });
            DropTable("dbo.EventTypeModels");
            DropTable("dbo.EventModels");
            DropTable("dbo.EventUserModels");
        }
    }
}
