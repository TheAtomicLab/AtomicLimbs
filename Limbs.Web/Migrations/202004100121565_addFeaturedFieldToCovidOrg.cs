namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFeaturedFieldToCovidOrg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CovidOrganizationModels", "Featured", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CovidOrganizationModels", "Featured");
        }
    }
}
