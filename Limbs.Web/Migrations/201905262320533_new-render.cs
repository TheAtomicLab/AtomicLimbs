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
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        PrimaryUrlImage = c.String(),
                        SecondaryUrlImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ColorModels",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        AmputationTypeId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        PrimaryUrlImage = c.String(),
                        SecondaryUrlImage = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmputationTypeModels", t => t.AmputationTypeId, cascadeDelete: true)
                .Index(t => t.AmputationTypeId);
            
            CreateTable(
                "dbo.RenderModels",
                c => new
                    {
                        Id = c.Int(nullable: false),
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
                        Id = c.Int(nullable: false),
                        RenderId = c.Int(nullable: false),
                        Name = c.String(),
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
            
            AddColumn("dbo.OrderModels", "AmputationTypeFkId", c => c.Int());
            AddColumn("dbo.OrderModels", "ColorFkId", c => c.Int());
            CreateIndex("dbo.OrderModels", "AmputationTypeFkId");
            CreateIndex("dbo.OrderModels", "ColorFkId");
            AddForeignKey("dbo.OrderModels", "ColorFkId", "dbo.ColorModels", "Id");
            AddForeignKey("dbo.OrderModels", "AmputationTypeFkId", "dbo.AmputationTypeModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RenderPieceModels", "RenderId", "dbo.RenderModels");
            DropForeignKey("dbo.OrderRenderPieceModels", "RenderPieceId", "dbo.RenderPieceModels");
            DropForeignKey("dbo.OrderRenderPieceModels", "OrderId", "dbo.OrderModels");
            DropForeignKey("dbo.RenderModels", "AmputationTypeId", "dbo.AmputationTypeModels");
            DropForeignKey("dbo.OrderModels", "AmputationTypeFkId", "dbo.AmputationTypeModels");
            DropForeignKey("dbo.OrderModels", "ColorFkId", "dbo.ColorModels");
            DropForeignKey("dbo.ColorModels", "AmputationTypeId", "dbo.AmputationTypeModels");
            DropIndex("dbo.OrderRenderPieceModels", new[] { "RenderPieceId" });
            DropIndex("dbo.OrderRenderPieceModels", new[] { "OrderId" });
            DropIndex("dbo.RenderPieceModels", new[] { "RenderId" });
            DropIndex("dbo.RenderModels", new[] { "AmputationTypeId" });
            DropIndex("dbo.ColorModels", new[] { "AmputationTypeId" });
            DropIndex("dbo.OrderModels", new[] { "ColorFkId" });
            DropIndex("dbo.OrderModels", new[] { "AmputationTypeFkId" });
            DropColumn("dbo.OrderModels", "ColorFkId");
            DropColumn("dbo.OrderModels", "AmputationTypeFkId");
            DropTable("dbo.OrderRenderPieceModels");
            DropTable("dbo.RenderPieceModels");
            DropTable("dbo.RenderModels");
            DropTable("dbo.ColorModels");
            DropTable("dbo.AmputationTypeModels");
        }
    }
}
