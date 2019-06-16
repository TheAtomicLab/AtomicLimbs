using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class AmputationTypeDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Amputación", Description = "")]
        [Required(ErrorMessage = "Campo requerido")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }
    }
}