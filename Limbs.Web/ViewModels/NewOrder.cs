using Limbs.Web.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class NewOrder
    {
        public string IdImage { get; set; }

        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public int AmputationTypeFkId { get; set; }
        public int? ColorFkId { get; set; }
        public string Comments { get; set; }
        public ProductType ProductType { get; set; }

        public bool HasDesign { get; set; }
    }
}