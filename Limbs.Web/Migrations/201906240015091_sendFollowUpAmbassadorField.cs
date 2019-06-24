using System;
using System.Data.Entity.Migrations;

namespace Limbs.Web.Migrations
{
    public partial class sendFollowUpAmbassadorField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "SendFollowUp", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AmbassadorModels", "SendFollowUp");
        }
    }
}
