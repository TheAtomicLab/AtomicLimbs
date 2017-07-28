namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDatePropIntoOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "Date");
        }
    }
}
