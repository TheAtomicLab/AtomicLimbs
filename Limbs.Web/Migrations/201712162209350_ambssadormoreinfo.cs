namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ambssadormoreinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmbassadorModels", "RegisteredAt", c => c.DateTime(defaultValueSql: "SYSDATETIME()"));
            AddColumn("dbo.AmbassadorModels", "State", c => c.String(nullable: true));
            AddColumn("dbo.AmbassadorModels", "Address2", c => c.String(nullable: true));

            Sql("update dbo.UserModels set RegisteredAt = SYSDATETIME()");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AmbassadorModels", "Address2");
            DropColumn("dbo.AmbassadorModels", "State");
            DropColumn("dbo.AmbassadorModels", "RegisteredAt");
        }
    }
}
