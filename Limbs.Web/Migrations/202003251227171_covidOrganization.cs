namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class covidOrganization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CovidOrganizationModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CovidOrganization = c.Int(nullable: false),
                        CovidOrganizationName = c.String(),
                        Name = c.String(),
                        Surname = c.String(),
                        Country = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Address = c.String(),
                        Address2 = c.String(),
                        Quantity = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        Token = c.String(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CovidOrganizationModels");
        }
    }
}
