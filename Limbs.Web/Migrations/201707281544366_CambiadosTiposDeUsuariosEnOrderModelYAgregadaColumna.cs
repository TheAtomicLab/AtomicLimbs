namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiadosTiposDeUsuariosEnOrderModelYAgregadaColumna : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderUser_Id" });
            AddColumn("dbo.AmbassadorModels", "UserId", c => c.String());
            AddColumn("dbo.OrderModels", "OrderAmbassador_Id", c => c.Int());
            AddColumn("dbo.UserModels", "UserId", c => c.String());
            AlterColumn("dbo.OrderModels", "OrderRequestor_Id", c => c.Int());
            CreateIndex("dbo.OrderModels", "OrderAmbassador_Id");
            CreateIndex("dbo.OrderModels", "OrderRequestor_Id");
            AddForeignKey("dbo.OrderModels", "OrderAmbassador_Id", "dbo.AmbassadorModels", "Id");
            DropColumn("dbo.OrderModels", "OrderUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "OrderUser_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.OrderModels", "OrderAmbassador_Id", "dbo.AmbassadorModels");
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderAmbassador_Id" });
            AlterColumn("dbo.OrderModels", "OrderRequestor_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.UserModels", "UserId");
            DropColumn("dbo.OrderModels", "OrderAmbassador_Id");
            DropColumn("dbo.AmbassadorModels", "UserId");
            CreateIndex("dbo.OrderModels", "OrderUser_Id");
            CreateIndex("dbo.OrderModels", "OrderRequestor_Id");
            AddForeignKey("dbo.OrderModels", "OrderUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
