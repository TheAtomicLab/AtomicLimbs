namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedEmbajadorUserWithEmbajadorRole : DbMigration
    {
        public override void Up()
        {
            Sql(@"
            INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'8af0fef9-842d-4d53-9d36-531e6ef1f390', N'embajador@limbs.com', 0, N'AJ7NxrCZjgiEO9yqgLLqx2ozG4XnWPDUxN/WWnmo0CxG4t/Z6NiEc15lv/FH/2cgLw==', N'b03ece7e-1332-4439-bed6-87a4594932e2', NULL, 0, 0, NULL, 1, 0, N'embajador@limbs.com')
            INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'875b9785-70c4-4a1e-8612-4fe8d23d060f', N'usuario@limbs.com', 0, N'AD0E9S48C8Y0naoNlbIx2Wv/IWAlM81fR0xDSrbB2ovtKTTCduWkDLdEz9eiTnVjxA==', N'749528d3-c0f3-49df-8780-7201c57350d2', NULL, 0, 0, NULL, 1, 0, N'usuario@limbs.com')
            
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'8af0fef9-842d-4d53-9d36-531e6ef1f390', N'9fdc4485-462c-427b-bf6b-05b178fd375e')
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'875b9785-70c4-4a1e-8612-4fe8d23d060f', N'35e9cbd7-d2b7-423d-a216-3f0f53e277fe')");
        }
        
        public override void Down()
        {
        }
    }
}
