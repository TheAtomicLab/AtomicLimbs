using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class CovidEmbajadorEntregableViewModel
    {
        public int Id { get; set; }
        public int AmbassadorId { get; set; }
        public int TipoEntregable { get; set; }

        [Required(ErrorMessage = " "), Range(0, int.MaxValue, ErrorMessage = " ")]
        public int CantEntregable { get; set; }
    }
}