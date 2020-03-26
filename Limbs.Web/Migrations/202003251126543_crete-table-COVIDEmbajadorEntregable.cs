namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cretetableCOVIDEmbajadorEntregable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.COVIDEmbajadorEntregables",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Ambassador_Id = c.Int(),
                    CantEntregable = c.Int(),
                    TipoEntregable = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmbassadorModels", t => t.Ambassador_Id)
                .Index(t => t.Ambassador_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.COVIDEmbajadorEntregables", "Ambassador_Id", "dbo.AmbassadorModels");
            DropIndex("dbo.COVIDEmbajadorEntregables", new[] { "Ambassador_Id" });
            DropTable("dbo.COVIDEmbajadorEntregables");
        }
    }
}
