namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderSizesAsFloat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderSizesModels", "A", c => c.Single(nullable: false));
            AlterColumn("dbo.OrderSizesModels", "B", c => c.Single(nullable: false));
            AlterColumn("dbo.OrderSizesModels", "C", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderSizesModels", "C", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderSizesModels", "B", c => c.Int(nullable: false));
            AlterColumn("dbo.OrderSizesModels", "A", c => c.Int(nullable: false));
        }
    }
}
