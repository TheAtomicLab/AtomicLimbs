using System.ComponentModel.DataAnnotations;
using Limbs.Web.Resources;

namespace Limbs.Web.ViewModels
{
    public class AmputationTypeDetailsViewModel
    {
        public int Id { get; set; }
       
        [Display(Name = "AmputationTypeDetails_Name", Description = "", ResourceType = typeof(ModelsTexts))]
        [Required(ErrorMessageResourceName = "RequiredErrorMessage", ErrorMessageResourceType = typeof(ModelsTexts))]
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }
    }
}