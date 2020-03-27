namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRelationBetweenOrgsAndAmbassadors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CovidOrgAmbassadorModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CovidAmbassadorId = c.Int(nullable: false),
                        CovidOrgId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.COVIDEmbajadorEntregables", t => t.CovidAmbassadorId, cascadeDelete: true)
                .ForeignKey("dbo.CovidOrganizationModels", t => t.CovidOrgId, cascadeDelete: true)
                .Index(t => t.CovidAmbassadorId)
                .Index(t => t.CovidOrgId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CovidOrgAmbassadorModels", "CovidOrgId", "dbo.CovidOrganizationModels");
            DropForeignKey("dbo.CovidOrgAmbassadorModels", "CovidAmbassadorId", "dbo.COVIDEmbajadorEntregables");
            DropIndex("dbo.CovidOrgAmbassadorModels", new[] { "CovidOrgId" });
            DropIndex("dbo.CovidOrgAmbassadorModels", new[] { "CovidAmbassadorId" });
            DropTable("dbo.CovidOrgAmbassadorModels");
        }
    }
}
