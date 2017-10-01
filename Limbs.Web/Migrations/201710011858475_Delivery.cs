namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "ProofOfDelivery", c => c.String());
            AddColumn("dbo.OrderModels", "DeliveryCourier", c => c.Int(nullable: false));
            AddColumn("dbo.OrderModels", "DeliveryTrackingCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "DeliveryTrackingCode");
            DropColumn("dbo.OrderModels", "DeliveryCourier");
            DropColumn("dbo.OrderModels", "ProofOfDelivery");
        }
    }
}
