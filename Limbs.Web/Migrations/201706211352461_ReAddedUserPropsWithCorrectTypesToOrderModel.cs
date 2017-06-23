namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddedUserPropsWithCorrectTypesFromOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "OrderRequestor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.OrderModels", "OrderUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.OrderModels", "OrderRequestor_Id");
            CreateIndex("dbo.OrderModels", "OrderUser_Id");
            AddForeignKey("dbo.OrderModels", "OrderRequestor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderModels", "OrderRequestor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.OrderModels", new[] { "OrderUser_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropColumn("dbo.OrderModels", "OrderUser_Id");
            DropColumn("dbo.OrderModels", "OrderRequestor_Id");
        }
    }
}
