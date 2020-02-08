namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createSponsorModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SponsorModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WebImage = c.String(),
                        MobileImage = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        Event_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventModels", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SponsorModels", "Event_Id", "dbo.EventModels");
            DropIndex("dbo.SponsorModels", new[] { "Event_Id" });
            DropTable("dbo.SponsorModels");
        }
    }
}
