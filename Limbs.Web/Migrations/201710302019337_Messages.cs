namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageModels",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        Priority = c.Int(nullable: false),
                        Content = c.String(),
                        Status = c.Int(nullable: false),
                        From_Id = c.String(maxLength: 128),
                        Order_Id = c.Int(),
                        PreviousMessage_Id = c.Guid(),
                        To_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.From_Id)
                .ForeignKey("dbo.OrderModels", t => t.Order_Id)
                .ForeignKey("dbo.MessageModels", t => t.PreviousMessage_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.To_Id)
                .Index(t => t.From_Id)
                .Index(t => t.Order_Id)
                .Index(t => t.PreviousMessage_Id)
                .Index(t => t.To_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageModels", "To_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageModels", "PreviousMessage_Id", "dbo.MessageModels");
            DropForeignKey("dbo.MessageModels", "Order_Id", "dbo.OrderModels");
            DropForeignKey("dbo.MessageModels", "From_Id", "dbo.AspNetUsers");
            DropIndex("dbo.MessageModels", new[] { "To_Id" });
            DropIndex("dbo.MessageModels", new[] { "PreviousMessage_Id" });
            DropIndex("dbo.MessageModels", new[] { "Order_Id" });
            DropIndex("dbo.MessageModels", new[] { "From_Id" });
            DropTable("dbo.MessageModels");
        }
    }
}
