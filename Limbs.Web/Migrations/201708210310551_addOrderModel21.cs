namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderModel21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "OrderModelId", c => c.Int(nullable: false));
            AddColumn("dbo.UserModels", "OrderModelId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "OrderModelId");
            DropColumn("dbo.AmbassadorModels", "OrderModelId");
        }
    }
}
