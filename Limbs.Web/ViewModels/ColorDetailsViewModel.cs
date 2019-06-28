using System.ComponentModel.DataAnnotations;
using Limbs.Web.Resources;

namespace Limbs.Web.ViewModels
{
    public class ColorDetailsViewModel
    {
        public int Id { get; set; }
        public int AmputationTypeId { get; set; }

        [Display(Name = "ColorDetails_Name_Name", Description = "ColorDetails_Name_Description", ResourceType = typeof(ModelsTexts))]

        public string Name { get; set; }

        public string Description { get; set; }

        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }
    }
}