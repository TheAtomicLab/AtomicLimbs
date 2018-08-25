namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class responsableDni : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserModels", "ResponsableDni", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "ResponsableDni");
        }
    }
}
