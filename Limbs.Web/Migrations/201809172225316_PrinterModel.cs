namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrinterModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AmbassadorModels", "Printer_Id", "dbo.PrinterModels");
            DropIndex("dbo.AmbassadorModels", new[] { "Printer_Id" });
            AddColumn("dbo.AmbassadorModels", "Printer_Width", c => c.Int(nullable: true));
            AddColumn("dbo.AmbassadorModels", "Printer_Height", c => c.Int(nullable: true));
            AddColumn("dbo.AmbassadorModels", "Printer_Long", c => c.Int(nullable: true));
            AddColumn("dbo.AmbassadorModels", "Printer_Brand", c => c.String());
            AddColumn("dbo.AmbassadorModels", "Printer_Model", c => c.String());
            AddColumn("dbo.AmbassadorModels", "Printer_PrintingArea", c => c.String());
            AddColumn("dbo.AmbassadorModels", "Printer_IsHotBed", c => c.Boolean(nullable: false));
            DropColumn("dbo.AmbassadorModels", "Printer_Id");
            DropTable("dbo.PrinterModels");
        }
        
        public override void Down()
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
                        PrintingArea = c.String(),
                        IsHotBed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AmbassadorModels", "Printer_Id", c => c.Int());
            DropColumn("dbo.AmbassadorModels", "Printer_IsHotBed");
            DropColumn("dbo.AmbassadorModels", "Printer_PrintingArea");
            DropColumn("dbo.AmbassadorModels", "Printer_Model");
            DropColumn("dbo.AmbassadorModels", "Printer_Brand");
            DropColumn("dbo.AmbassadorModels", "Printer_Long");
            DropColumn("dbo.AmbassadorModels", "Printer_Height");
            DropColumn("dbo.AmbassadorModels", "Printer_Width");
            CreateIndex("dbo.AmbassadorModels", "Printer_Id");
            AddForeignKey("dbo.AmbassadorModels", "Printer_Id", "dbo.PrinterModels", "Id");
        }
    }
}
