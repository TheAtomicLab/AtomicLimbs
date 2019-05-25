using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.Entities.Models
{
    public class OrderRenderPieceModel
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int RenderPieceId { get; set; }
        public bool Printed { get; set; }

        public RenderPieceModel RenderPiece { get; set; }
        public OrderModel Order { get; set; }
    }
}
