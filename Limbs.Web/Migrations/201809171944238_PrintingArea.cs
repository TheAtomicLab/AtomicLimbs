namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingArea : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrinterModels", "PrintingArea", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrinterModels", "PrintingArea");
        }
    }
}
