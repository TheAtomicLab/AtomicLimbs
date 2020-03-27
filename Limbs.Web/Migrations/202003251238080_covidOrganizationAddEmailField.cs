namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class covidOrganizationAddEmailField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CovidOrganizationModels", "Email", c => c.String());
            AddColumn("dbo.CovidOrganizationModels", "OrganizationPhone", c => c.String());
            AddColumn("dbo.CovidOrganizationModels", "OrganizationPhoneIntern", c => c.String());
            AddColumn("dbo.CovidOrganizationModels", "PersonalPhone", c => c.String());
            AddColumn("dbo.CovidOrganizationModels", "Dni", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CovidOrganizationModels", "Email");
            DropColumn("dbo.CovidOrganizationModels", "PersonalPhone");
            DropColumn("dbo.CovidOrganizationModels", "OrganizationPhone");
            DropColumn("dbo.CovidOrganizationModels", "OrganizationPhoneIntern");
            DropColumn("dbo.CovidOrganizationModels", "Dni");
        }
    }
}
