namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class covidOrganizationAddEmailField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CovidOrganizationModels", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CovidOrganizationModels", "Email");
        }
    }
}
