using Limbs.Web.Entities.Models;
using Limbs.Web.Resources;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class NewOrder
    {
        public string IdImage { get; set; }

        [Display(Name = "NewOrder_AmputationTypeFkId", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessage = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelsTexts))]
        public int AmputationTypeFkId { get; set; }
        public int? ColorFkId { get; set; }
        public string Comments { get; set; }
        public ProductType ProductType { get; set; }

        public bool HasDesign { get; set; }
    }
}