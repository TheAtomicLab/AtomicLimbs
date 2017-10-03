namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeliveryLabelAndOrderLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "DeliveryPostalLabel", c => c.String());
            AddColumn("dbo.OrderModels", "OrderLog", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "OrderLog");
            DropColumn("dbo.OrderModels", "DeliveryPostalLabel");
        }
    }
}
