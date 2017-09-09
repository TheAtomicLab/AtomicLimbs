namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class holitas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmbassadorModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Email = c.String(),
                        AmbassadorName = c.String(nullable: false),
                        AmbassadorLastName = c.String(nullable: false),
                        Birth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        Country = c.String(nullable: false),
                        City = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Dni = c.String(nullable: false),
                        Lat = c.Double(nullable: false),
                        Long = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Design = c.Int(nullable: false),
                        Comments = c.String(),
                        IdImage = c.String(),
                        Status = c.Int(nullable: false),
                        StatusLastUpdated = c.DateTime(nullable: false),
                        OrderAmbassador_Id = c.Int(),
                        OrderRequestor_Id = c.Int(),
                        Sizes_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmbassadorModels", t => t.OrderAmbassador_Id)
                .ForeignKey("dbo.UserModels", t => t.OrderRequestor_Id)
                .ForeignKey("dbo.OrderSizesModels", t => t.Sizes_Id)
                .Index(t => t.OrderAmbassador_Id)
                .Index(t => t.OrderRequestor_Id)
                .Index(t => t.Sizes_Id);
            
            CreateTable(
                "dbo.AccessoryModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.String(),
                        ImageUrl = c.String(),
                        OrderModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderModels", t => t.OrderModel_Id)
                .Index(t => t.OrderModel_Id);
            
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        Accessory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccessoryModels", t => t.Accessory_Id)
                .Index(t => t.Accessory_Id);
            
            CreateTable(
                "dbo.UserModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Email = c.String(),
                        UserName = c.String(nullable: false),
                        UserLastName = c.String(nullable: false),
                        ResponsableName = c.String(nullable: false),
                        ResponsableLastName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Birth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        Country = c.String(nullable: false),
                        City = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        Dni = c.String(nullable: false),
                        ProthesisType = c.Int(nullable: false),
                        ProductType = c.Int(nullable: false),
                        AmputationType = c.Int(nullable: false),
                        Lat = c.Double(nullable: false),
                        Long = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderSizesModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        A = c.Int(nullable: false),
                        B = c.Int(nullable: false),
                        C = c.Int(nullable: false),
                        D = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Thumbnail = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OrderModels", "Sizes_Id", "dbo.OrderSizesModels");
            DropForeignKey("dbo.OrderModels", "OrderRequestor_Id", "dbo.UserModels");
            DropForeignKey("dbo.OrderModels", "OrderAmbassador_Id", "dbo.AmbassadorModels");
            DropForeignKey("dbo.AccessoryModels", "OrderModel_Id", "dbo.OrderModels");
            DropForeignKey("dbo.Colors", "Accessory_Id", "dbo.AccessoryModels");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Colors", new[] { "Accessory_Id" });
            DropIndex("dbo.AccessoryModels", new[] { "OrderModel_Id" });
            DropIndex("dbo.OrderModels", new[] { "Sizes_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderRequestor_Id" });
            DropIndex("dbo.OrderModels", new[] { "OrderAmbassador_Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ProductModels");
            DropTable("dbo.OrderSizesModels");
            DropTable("dbo.UserModels");
            DropTable("dbo.Colors");
            DropTable("dbo.AccessoryModels");
            DropTable("dbo.OrderModels");
            DropTable("dbo.AmbassadorModels");
        }
    }
}
