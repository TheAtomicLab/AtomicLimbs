namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedRoles : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'a0906d81-0cd9-43d6-8c14-8a18d61caa2f', N'Unassigned')");
            Sql("INSERT INTO[dbo].[AspNetRoles] ([Id], [Name]) VALUES(N'78f7eb27-10b1-4b73-9fa6-2a8f5356e163', N'Requester')");
            Sql("INSERT INTO[dbo].[AspNetRoles]([Id], [Name]) VALUES(N'020d80d0-98c5-4bfb-b9a2-6e3f1e22f82d', N'Ambassador')");
            Sql("INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'c9beaaae-ee3c-46c9-b446-88b20f2647ac', N'Admin')");
        }
        
        public override void Down()
        {
        }
    }
}
