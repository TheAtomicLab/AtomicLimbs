namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeDeliveryDateField : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CovidOrganizationModels", "DeliveryDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CovidOrganizationModels", "DeliveryDate", c => c.DateTime(nullable: false));
        }
    }
}
