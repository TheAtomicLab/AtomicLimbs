using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class RenderPieceModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RenderId { get; set; }
        public int Name { get; set; }

        public List<OrderRenderPieceModel> Orders { get; set; }
        public RenderModel Render { get; set; }
    }
}
