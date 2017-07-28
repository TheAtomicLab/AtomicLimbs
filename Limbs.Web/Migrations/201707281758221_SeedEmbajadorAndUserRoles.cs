namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedEmbajadorAndUserRoles : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'9fdc4485-462c-427b-bf6b-05b178fd375e', N'Embajador')");
            Sql("INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'35e9cbd7-d2b7-423d-a216-3f0f53e277fe', N'Usuario')");
        }
        
        public override void Down()
        {
        }
    }
}
