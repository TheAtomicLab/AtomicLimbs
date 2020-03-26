using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Limbs.Web.Entities.Models
{
    public class COVIDEmbajadorEntregable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public AmbassadorModel Ambassador { get; set; }
        public int TipoEntregable { get; set; }
        public int CantEntregable { get; set; }
    }

    public enum TipoEntregable
    {
        Mascara = 1,
    }
}
