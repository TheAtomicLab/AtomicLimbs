namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempRemovedUserPropsFromOrderModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderModels", "OrderRequestor_Id", "dbo.UserModels");
            DropForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.UserModels");
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderUser_Id" });
            DropColumn("dbo.OrderModels", "OrderRequestor_Id");
            DropColumn("dbo.OrderModels", "OrderUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "OrderUser_Id", c => c.Int());
            AddColumn("dbo.OrderModels", "OrderRequestor_Id", c => c.Int());
            CreateIndex("dbo.OrderModels", "OrderUser_Id");
            CreateIndex("dbo.OrderModels", "OrderRequestor_Id");
            AddForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.UserModels", "Id");
            AddForeignKey("dbo.OrderModels", "OrderRequestor_Id", "dbo.UserModels", "Id");
        }
    }
}
