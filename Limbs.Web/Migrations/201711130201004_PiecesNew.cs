namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PiecesNew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "Pieces_Thumb", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_ThumbClip", c => c.Boolean(nullable: false));
            DropColumn("dbo.OrderModels", "Pieces_ThumbThumbClip");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderModels", "Pieces_ThumbThumbClip", c => c.Boolean(nullable: false));
            DropColumn("dbo.OrderModels", "Pieces_ThumbClip");
            DropColumn("dbo.OrderModels", "Pieces_Thumb");
        }
    }
}
