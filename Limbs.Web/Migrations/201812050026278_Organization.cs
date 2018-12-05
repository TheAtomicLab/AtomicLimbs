namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Organization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "Organization", c => c.Int(nullable: false));
            AddColumn("dbo.AmbassadorModels", "OrganizationName", c => c.String());
            AddColumn("dbo.AmbassadorModels", "RoleInOrganization", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AmbassadorModels", "RoleInOrganization");
            DropColumn("dbo.AmbassadorModels", "OrganizationName");
            DropColumn("dbo.AmbassadorModels", "Organization");
        }
    }
}
