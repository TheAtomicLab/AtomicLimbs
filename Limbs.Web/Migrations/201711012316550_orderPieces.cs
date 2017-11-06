namespace Limbs.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderPieces : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderModels", "Pieces_AtomicLabCover", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_FingerMechanismHolder", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_Fingers", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_FingerStopper", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_FingersX1", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_FingersX2P", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_Palm", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_ThumbConnector", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_ThumbThumbClip", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_ThumbScrew", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_UpperArm_FingerConnector", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_UpperArm_PalmConnector", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_UpperArm_ThumbShortConnector", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_UpperArm_FingerSlider", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderModels", "Pieces_UpperArm", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderModels", "Pieces_UpperArm");
            DropColumn("dbo.OrderModels", "Pieces_UpperArm_FingerSlider");
            DropColumn("dbo.OrderModels", "Pieces_UpperArm_ThumbShortConnector");
            DropColumn("dbo.OrderModels", "Pieces_UpperArm_PalmConnector");
            DropColumn("dbo.OrderModels", "Pieces_UpperArm_FingerConnector");
            DropColumn("dbo.OrderModels", "Pieces_ThumbScrew");
            DropColumn("dbo.OrderModels", "Pieces_ThumbThumbClip");
            DropColumn("dbo.OrderModels", "Pieces_ThumbConnector");
            DropColumn("dbo.OrderModels", "Pieces_Palm");
            DropColumn("dbo.OrderModels", "Pieces_FingersX2P");
            DropColumn("dbo.OrderModels", "Pieces_FingersX1");
            DropColumn("dbo.OrderModels", "Pieces_FingerStopper");
            DropColumn("dbo.OrderModels", "Pieces_Fingers");
            DropColumn("dbo.OrderModels", "Pieces_FingerMechanismHolder");
            DropColumn("dbo.OrderModels", "Pieces_AtomicLabCover");
        }
    }
}
