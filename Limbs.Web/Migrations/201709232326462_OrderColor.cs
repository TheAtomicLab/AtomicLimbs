namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderColor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "Color", c => c.Int(nullable: false));
            DropColumn("dbo.OrderModels", "Design");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "Design", c => c.Int(nullable: false));
            DropColumn("dbo.OrderModels", "Color");
        }
    }
}
