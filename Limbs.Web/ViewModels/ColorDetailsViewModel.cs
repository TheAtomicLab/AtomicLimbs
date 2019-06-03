using System.ComponentModel.DataAnnotations;

namespace Limbs.Web.ViewModels
{
    public class ColorDetailsViewModel
    {
        public int Id { get; set; }
        public int AmputationTypeId { get; set; }

        [Display(Name = "Color", Description = "(si es posible)")]

        public string Name { get; set; }

        public string Description { get; set; }

        public string PrimaryUrlImage { get; set; }
        public string SecondaryUrlImage { get; set; }
    }
}