namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductFileUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "FileUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "FileUrl");
        }
    }
}
