namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "OrderModelId", c => c.Byte(nullable: false));
            AddColumn("dbo.UserModels", "OrderModelId", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "OrderModelId");
            DropColumn("dbo.AmbassadorModels", "OrderModelId");
        }
    }
}
