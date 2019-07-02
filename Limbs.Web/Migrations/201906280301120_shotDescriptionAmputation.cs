namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shotDescriptionAmputation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmputationTypeModels", "Short_Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AmputationTypeModels", "Short_Description");
        }
    }
}
