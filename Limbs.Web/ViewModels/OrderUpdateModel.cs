using Limbs.Web.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class OrderUpdateModel
    {
        public int Id { get; set; }

        [Display(Name = "¿Cuál?", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Comentarios", Description = "")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Color", Description = "(si es posible)")]
        public int? ColorFkId { get; set; }

        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public int AmputationTypeFkId { get; set; }

        public int PreviousAmputationTypeId { get; set; }

        public string[] Images { get; set; }
        public int TotalImages { get; set; }

        public bool HasDesign { get; set; }
    }

    public class OrderDeleteImage
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string FileNameBlob { get; set; }
    }
}