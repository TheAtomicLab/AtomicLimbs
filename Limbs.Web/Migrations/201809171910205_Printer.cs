namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Printer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrinterModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Long = c.Int(nullable: false),
                        Brand = c.String(),
                        Model = c.String(),
                        IsHotBed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AmbassadorModels", "Printer_Id", c => c.Int());
            AlterColumn("dbo.UserModels", "ResponsableDni", c => c.String());
            CreateIndex("dbo.AmbassadorModels", "Printer_Id");
            AddForeignKey("dbo.AmbassadorModels", "Printer_Id", "dbo.PrinterModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AmbassadorModels", "Printer_Id", "dbo.PrinterModels");
            DropIndex("dbo.AmbassadorModels", new[] { "Printer_Id" });
            AlterColumn("dbo.UserModels", "ResponsableDni", c => c.String(nullable: false));
            DropColumn("dbo.AmbassadorModels", "Printer_Id");
            DropTable("dbo.PrinterModels");
        }
    }
}
