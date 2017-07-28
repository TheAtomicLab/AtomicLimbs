namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE[OrderModels] DROP CONSTRAINT[FK_dbo.OrderModels_dbo.UserModels_OrderRequestor_Id]");
            Sql("ALTER TABLE[OrderModels] DROP CONSTRAINT[FK_dbo.OrderModels_dbo.UserModels_OrderUser_Id]");


            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderUser_Id" });

            AlterColumn("dbo.OrderModels", "OrderRequestor_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.OrderModels", "OrderUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.OrderModels", "OrderRequestor_Id");
            CreateIndex("dbo.OrderModels", "OrderUser_Id");
            DropColumn("dbo.UserModels", "Trava");
            DropColumn("dbo.UserModels", "Country");
            DropColumn("dbo.UserModels", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserModels", "City", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "Country", c => c.String(nullable: false));
            AddColumn("dbo.UserModels", "Trava", c => c.Int(nullable: false));
            DropIndex("dbo.OrderModels", new[] { "OrderUser_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            AlterColumn("dbo.OrderModels", "OrderUser_Id", c => c.Int());
            AlterColumn("dbo.OrderModels", "OrderRequestor_Id", c => c.Int());

            CreateIndex("dbo.OrderModels", "OrderUser_Id");
            CreateIndex("dbo.OrderModels", "OrderRequestor_Id");
        }
    }
}
