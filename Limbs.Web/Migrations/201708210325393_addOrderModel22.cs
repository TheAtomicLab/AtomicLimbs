namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderModel22 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AmbassadorModels", "OrderModelId");
            DropColumn("dbo.UserModels", "OrderModelId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserModels", "OrderModelId", c => c.Int(nullable: false));
            AddColumn("dbo.AmbassadorModels", "OrderModelId", c => c.Int(nullable: false));
        }
    }
}
