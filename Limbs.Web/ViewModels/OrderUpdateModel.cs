using Limbs.Web.Entities.Models;
using Limbs.Web.Resources;
using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class OrderUpdateModel
    {
        public int Id { get; set; }

        [Display(Name = "OrderUpdate_ProductType", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelsTexts))]
        public ProductType ProductType { get; set; }

        [Display(Name = "OrderUpdate_Comments", Description = "", ResourceType = typeof(ModelsTexts))]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "OrderUpdate_ColorFkId_Name", Description = "OrderUpdate_ColorFkId_Description", ResourceType = typeof(ModelsTexts))]
        public int? ColorFkId { get; set; }

        [Display(Name = "OrderUpdate_AmputationTypeFkId", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelsTexts))]
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