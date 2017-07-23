namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "Thumbnail", c => c.String(nullable: false));
            AlterColumn("dbo.ProductModels", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ProductModels", "Type", c => c.String(nullable: false));
            DropColumn("dbo.ProductModels", "IsRightHand");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductModels", "IsRightHand", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ProductModels", "Type", c => c.String());
            AlterColumn("dbo.ProductModels", "Name", c => c.String());
            DropColumn("dbo.ProductModels", "Thumbnail");
        }
    }
}
