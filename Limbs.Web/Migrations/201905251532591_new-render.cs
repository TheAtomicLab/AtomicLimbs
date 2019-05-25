namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newrender : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmputationTypeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        PrimaryUrlImage = c.String(),
                        SecondaryUrlImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AmputationTypeColorModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AmputationTypeId = c.Int(nullable: false),
                        ColorId = c.Int(nullable: false),
                        PrimaryUrlImage = c.String(),
                        SecondaryUrlImage = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmputationTypeModels", t => t.AmputationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ColorModels", t => t.ColorId, cascadeDelete: true)
                .Index(t => t.AmputationTypeId)
                .Index(t => t.ColorId);
            
            CreateTable(
                "dbo.ColorModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AmputationTypeModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmputationTypeModels", t => t.AmputationTypeModel_Id)
                .Index(t => t.AmputationTypeModel_Id);
            
            CreateTable(
                "dbo.RenderModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AmputationTypeId = c.Int(nullable: false),
                        Name = c.String(),
                        PrimaryUrlImage = c.String(),
                        SecondaryUrlImage = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmputationTypeModels", t => t.AmputationTypeId, cascadeDelete: true)
                .Index(t => t.AmputationTypeId);
            
            CreateTable(
                "dbo.RenderPieceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RenderId = c.Int(nullable: false),
                        Name = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RenderModels", t => t.RenderId, cascadeDelete: true)
                .Index(t => t.RenderId);
            
            CreateTable(
                "dbo.OrderRenderPieceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        RenderPieceId = c.Int(nullable: false),
                        Printed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderModels", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.RenderPieceModels", t => t.RenderPieceId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.RenderPieceId);
            
            AddColumn("dbo.OrderModels", "AmputationTypeNew_Id", c => c.Int());
            CreateIndex("dbo.OrderModels", "AmputationTypeNew_Id");
            AddForeignKey("dbo.OrderModels", "AmputationTypeNew_Id", "dbo.AmputationTypeModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderModels", "AmputationTypeNew_Id", "dbo.AmputationTypeModels");
            DropForeignKey("dbo.RenderPieceModels", "RenderId", "dbo.RenderModels");
            DropForeignKey("dbo.OrderRenderPieceModels", "RenderPieceId", "dbo.RenderPieceModels");
            DropForeignKey("dbo.OrderRenderPieceModels", "OrderId", "dbo.OrderModels");
            DropForeignKey("dbo.RenderModels", "AmputationTypeId", "dbo.AmputationTypeModels");
            DropForeignKey("dbo.ColorModels", "AmputationTypeModel_Id", "dbo.AmputationTypeModels");
            DropForeignKey("dbo.AmputationTypeColorModels", "ColorId", "dbo.ColorModels");
            DropForeignKey("dbo.AmputationTypeColorModels", "AmputationTypeId", "dbo.AmputationTypeModels");
            DropIndex("dbo.OrderRenderPieceModels", new[] { "RenderPieceId" });
            DropIndex("dbo.OrderRenderPieceModels", new[] { "OrderId" });
            DropIndex("dbo.RenderPieceModels", new[] { "RenderId" });
            DropIndex("dbo.RenderModels", new[] { "AmputationTypeId" });
            DropIndex("dbo.ColorModels", new[] { "AmputationTypeModel_Id" });
            DropIndex("dbo.AmputationTypeColorModels", new[] { "ColorId" });
            DropIndex("dbo.AmputationTypeColorModels", new[] { "AmputationTypeId" });
            DropIndex("dbo.OrderModels", new[] { "AmputationTypeNew_Id" });
            DropColumn("dbo.OrderModels", "AmputationTypeNew_Id");
            DropTable("dbo.OrderRenderPieceModels");
            DropTable("dbo.RenderPieceModels");
            DropTable("dbo.RenderModels");
            DropTable("dbo.ColorModels");
            DropTable("dbo.AmputationTypeColorModels");
            DropTable("dbo.AmputationTypeModels");
        }
    }
}
