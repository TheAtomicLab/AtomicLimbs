using System.Collections.Generic;

namespace Limbs.Web.ViewModels
{
    public class RenderPieceGroupByViewModel
    {
        public RenderViewModel Render { get; set; }
        public List<OrderRenderPieceViewModel> OrderRenderPieces { get; set; }
    }
}