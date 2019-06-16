namespace Limbs.Web.ViewModels
{
    public class OrderRenderPieceViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int RenderPieceId { get; set; }
        public string PieceName { get; set; }
        public bool Printed { get; set; }
    }
}