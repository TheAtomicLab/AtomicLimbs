namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUpdateDatePropToOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "UpdateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "UpdateDate");
        }
    }
}
