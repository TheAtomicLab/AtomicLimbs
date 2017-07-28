namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedpropname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "StatusLastUpdated", c => c.DateTime(nullable: false));
            DropColumn("dbo.OrderModels", "UpdateDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "UpdateDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.OrderModels", "StatusLastUpdated");
        }
    }
}
