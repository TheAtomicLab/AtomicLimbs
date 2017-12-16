namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class moreuserdata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserModels", "RegisteredAt", c => c.DateTime(defaultValueSql: "SYSDATETIME()"));
            AddColumn("dbo.UserModels", "State", c => c.String(nullable: true));
            AddColumn("dbo.UserModels", "Address2", c => c.String(nullable: true));

            Sql("update dbo.UserModels set RegisteredAt = SYSDATETIME()");
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "Address2");
            DropColumn("dbo.UserModels", "State");
            DropColumn("dbo.UserModels", "RegisteredAt");
        }
    }
}
