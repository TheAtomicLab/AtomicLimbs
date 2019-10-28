namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixEventTypeFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EventModels", "EventyType_Id", "dbo.EventTypeModels");
            DropIndex("dbo.EventModels", new[] { "EventyType_Id" });
            DropColumn("dbo.EventModels", "EventTypeId");
            RenameColumn(table: "dbo.EventModels", name: "EventyType_Id", newName: "EventTypeId");
            AlterColumn("dbo.EventModels", "EventTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.EventModels", "EventTypeId");
            AddForeignKey("dbo.EventModels", "EventTypeId", "dbo.EventTypeModels", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventModels", "EventTypeId", "dbo.EventTypeModels");
            DropIndex("dbo.EventModels", new[] { "EventTypeId" });
            AlterColumn("dbo.EventModels", "EventTypeId", c => c.Int());
            RenameColumn(table: "dbo.EventModels", name: "EventTypeId", newName: "EventyType_Id");
            AddColumn("dbo.EventModels", "EventTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.EventModels", "EventyType_Id");
            AddForeignKey("dbo.EventModels", "EventyType_Id", "dbo.EventTypeModels", "Id");
        }
    }
}
