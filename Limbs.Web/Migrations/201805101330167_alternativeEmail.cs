namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alternativeEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "AlternativeEmail", c => c.String());
            AddColumn("dbo.UserModels", "AlternativeEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "AlternativeEmail");
            DropColumn("dbo.AmbassadorModels", "AlternativeEmail");
        }
    }
}
