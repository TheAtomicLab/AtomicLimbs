namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambiosuser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserModels", "Lat", c => c.Double(nullable: false));
            AlterColumn("dbo.UserModels", "Long", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserModels", "Long", c => c.Long(nullable: false));
            AlterColumn("dbo.UserModels", "Lat", c => c.Long(nullable: false));
        }
    }
}
